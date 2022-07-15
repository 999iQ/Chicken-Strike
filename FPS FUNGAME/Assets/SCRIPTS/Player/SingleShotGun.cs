using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotGun : Gun
{
    [Header("�������������� ������")]
    public int Ammo;
    public int AmmoInMagazin; // const
    public int AllAmmo;

    public float ReloadTime;
    private bool reloadFlag = false;

    public float timeBTWshot;
    private float StartTimeBTWshot;

    public AudioClip shotSFX;
    public AudioSource _audioSource;

    [SerializeField] Camera cam;
    PhotonView photonView;


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        StartTimeBTWshot = timeBTWshot;
        Ammo = AmmoInMagazin;
    }
    private void Update()
    {
        timeBTWshot -= Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.R) && reloadFlag != true) // reload
        {
            StartCoroutine(ReloadCoroutine(Ammo));
        }
    }
    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        if(timeBTWshot <= 0 && Ammo > 0 && reloadFlag != true)
        {
            timeBTWshot = StartTimeBTWshot;
            Ammo--;

            _audioSource.PlayOneShot(shotSFX);

            // ������� �� �������� ������
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            ray.origin = cam.transform.position;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage, PhotonNetwork.LocalPlayer.NickName);
                photonView.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            }
        }

    }

    private IEnumerator ReloadCoroutine(int countAmmo)
    {
        // ����
        // ***
        if (countAmmo != AmmoInMagazin)
        {
            reloadFlag = true;
            //����������� ���������� ���� � ����� ���� ���� ����� ����
            AllAmmo += countAmmo;
            Ammo = 0;

            yield return new WaitForSeconds(ReloadTime);

            if(AllAmmo >= AmmoInMagazin)
            {
                Ammo += AmmoInMagazin;
                AllAmmo -= AmmoInMagazin;
            }
            else
            {
                Ammo += AllAmmo; // ���� �������� ������ ��� � �������� ������ ��������� ��������� �������
                AllAmmo = 0;
            }

            reloadFlag = false;
            
        }

    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f); // ����� ���������� ��� ������������ ������ �� ���� � ������
        if(colliders.Length != 0)
        {
            // ������ ����� �� ���� � ��������� ������� ����� �������� �� �������������
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f,
            Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);

            Destroy(bulletImpactObj, 10f);

            bulletImpactObj.transform.SetParent(colliders[0].transform); // ����������� ������ �������� � �����
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotGun : Gun
{
    [Header("Хар-ки оружия")]
    public int Ammo;
    public int AmmoInMagazin; // const
    public int AllAmmo;

    public float aimSpeed; // скорость прицеливания

    public float ReloadTime;
    private bool reloadFlag = false;

    public float timeBTWshot;
    private float StartTimeBTWshot;

    public AudioClip shotSFX;
    public AudioSource _audioSource;

    [SerializeField] private Camera cam;
    PhotonView photonView;

    [SerializeField] private Transform _aimPosition;
    [SerializeField] private Transform _defaultPosition;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        StartTimeBTWshot = timeBTWshot;
        Ammo = AmmoInMagazin;
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        timeBTWshot -= Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.R) && reloadFlag != true) // reload
        {
            StartCoroutine(ReloadCoroutine(Ammo));
        }
    }

    public override void Aim(bool _isAiming)
    {
        if (_isAiming)
        {
            // из текущего положения в положение прицеливания
            itemGameObject.transform.position = Vector3.Lerp(_aimPosition.position, itemGameObject.transform.position, Time.deltaTime * aimSpeed);
        }
        else
        {
            // из текущего положения в дефолт от бедра
            itemGameObject.transform.position = Vector3.Lerp(_defaultPosition.position, itemGameObject.transform.position, Time.deltaTime * aimSpeed);
        }
    }
    public override void Use() // переопределение метода стрельбы
    {
        Shoot();
    }

    private void Shoot()
    {
        if(timeBTWshot <= 0 && Ammo > 0 && reloadFlag != true)
        {
            timeBTWshot = StartTimeBTWshot;
            Ammo--;

            _audioSource.PlayOneShot(shotSFX);

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
        // корутина перезарядки
        if (countAmmo != AmmoInMagazin)
        {
            reloadFlag = true;
           
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
                Ammo += AllAmmo; 
                AllAmmo = 0;
            }

            reloadFlag = false;
            
        }

    }

    [PunRPC]
    private void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f); // добавляет коллайдеры в массив для усыновения в радиусе сферы 
        if(colliders.Length != 0)
        {
            // создаём префаб попадания на объекте в который попали
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f,
            Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);

            Destroy(bulletImpactObj, 10f);

            bulletImpactObj.transform.SetParent(colliders[0].transform); //усыновляем префаб попадания чтобы он прилип к объекту :3
        }
    }

}

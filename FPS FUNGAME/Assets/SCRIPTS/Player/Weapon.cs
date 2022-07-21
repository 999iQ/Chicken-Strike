using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviourPunCallbacks
{
    public float damage, startBtwTime; //���� // �������
    private float _btwTime; // ��������
    public float range, force; // ��������� // ���� ���������
    public ParticleSystem muzzleFlash; // ������ ��������
    public AudioClip shotSFX;
    public AudioSource audioSource;
    [SerializeField] private GameObject _hitEffect;

    // RayCast
    [SerializeField] private GameObject _cam;

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if(_cam != null)
        {
            _btwTime -= Time.deltaTime;

            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (_btwTime <= 0)
                {
                    Shoot();
                    _btwTime = startBtwTime;
                }
            }
        }

    }
    private void Shoot()
    {
        audioSource.PlayOneShot(shotSFX);
        muzzleFlash.Play();

        // ������ ������� (�����, �����������, ����, ����� �����, ���� �������)
        Debug.DrawRay(_cam.transform.position,transform.forward, Color.red, 10f);

        RaycastHit hit;
        if (Physics.Raycast(_cam.transform.position, transform.forward, out hit, range))
        {
            GameObject impact = PhotonNetwork.Instantiate(_hitEffect.name, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 3f);

            if(hit.collider.tag == "Player")
            {
                hit.transform.GetComponent<PhotonView>().RPC("AddDamage", RpcTarget.All, damage);
            }

        }
    }

}

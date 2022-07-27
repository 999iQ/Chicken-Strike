using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SwayWeapon : MonoBehaviourPunCallbacks
{
    // метод плавно двигает оружие при движении камеры
    public float intensity;
    public float smooth;

    private Quaternion origin_rotation; // константа (исходное положение оружия)

    private void Start()
    {
        if (!photonView.IsMine)
            Destroy(this);

        origin_rotation = transform.localRotation;
    }

    private void Update()
    {
        UpdateSway();
    }

    private void UpdateSway()
    {
        //controls
        float t_x_mouse = Input.GetAxis("Mouse X");
        float t_y_mouse = Input.GetAxis("Mouse Y");

        //calculate target rotation
        Quaternion t_x_adj = Quaternion.AngleAxis(-intensity * t_x_mouse, Vector3.up); // x
        Quaternion t_y_adj = Quaternion.AngleAxis(intensity * t_y_mouse, Vector3.right); // y
        Quaternion target_rotation = origin_rotation * t_x_adj * t_y_adj; // складываем вектора умножением

        // rotate towards target rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, target_rotation, Time.deltaTime * smooth);
    }

}

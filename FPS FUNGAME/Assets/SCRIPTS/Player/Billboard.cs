using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // скрипт который поворачивает ник игрока к камерам других игроков

    Camera cam;

    private void Update()
    {
        if (cam == null)
            cam = FindObjectOfType<Camera>();

        if (cam == null)
            return;

        transform.LookAt(cam.transform);
        
    }
}

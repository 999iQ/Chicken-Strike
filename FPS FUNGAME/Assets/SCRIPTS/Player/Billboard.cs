using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // скрипт который поворачивает ник игрока к камерам других игроков
    private Camera _cam;

    private void Update()
    {
        if (_cam == null)
            _cam = FindObjectOfType<Camera>();

        if (_cam == null)
            return;

        transform.LookAt(_cam.transform);
        
    }
}

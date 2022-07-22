using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // скрипт поворачивает объект к игроку (локально) (следит за тобой)
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

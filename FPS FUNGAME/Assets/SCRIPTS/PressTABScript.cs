using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressTABScript : MonoBehaviour
{
    public GameObject ScoreBoardCanvas;
    private CanvasGroup _canvasGroup;

    private void Start()
    {
        _canvasGroup = gameObject.GetComponentInChildren<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            _canvasGroup.alpha = 1;
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            _canvasGroup.alpha = 0;
        }
        

    }

}

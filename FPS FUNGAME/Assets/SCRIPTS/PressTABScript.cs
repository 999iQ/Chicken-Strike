using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressTABScript : MonoBehaviour
{
    private GameManager _gameManager;
    private CanvasGroup _canvasGroup;

    private void Start()
    {
        _canvasGroup = gameObject.GetComponentInChildren<CanvasGroup>();
        _canvasGroup.alpha = 0;

        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            _gameManager.SendPlayerList();
            _canvasGroup.alpha = 1;
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            _canvasGroup.alpha = 0;
        }
    }
}

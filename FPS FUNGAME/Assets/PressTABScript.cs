using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressTABScript : MonoBehaviour
{
    public GameObject ScoreBoardCanvas;

    void Start()
    {
        ScoreBoardCanvas.SetActive(false);
    }

    private void Update()
    {
        // �� ������� ��� �����
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ScoreBoardCanvas.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            ScoreBoardCanvas.SetActive(false);
        }
        

    }

}

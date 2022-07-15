using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] PhotonView photonView;
    [SerializeField] TMP_Text TMPtext;

    private void Start()
    {
        if(photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
        TMPtext.text = photonView.Owner.NickName; // получаем ник 
    }

}

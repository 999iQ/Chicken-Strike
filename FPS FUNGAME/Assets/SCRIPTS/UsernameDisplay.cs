using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] PhotonView _photonView;
    [SerializeField] TMP_Text _TMPtext;

    private void Start()
    {
        if(_photonView.IsMine)
        {
            // отключаем свой ник
            gameObject.SetActive(false);
        }
        _TMPtext.text = _photonView.Owner.NickName; // получаем ник 
    }

}

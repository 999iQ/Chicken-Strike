using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private TMP_Text _TMPtext;

    private void Start()
    {
        if(_photonView.IsMine)
        {
            // отключаем свой ник у себя
            gameObject.SetActive(false);
        }
        _TMPtext.text = _photonView.Owner.NickName; 
    }

}

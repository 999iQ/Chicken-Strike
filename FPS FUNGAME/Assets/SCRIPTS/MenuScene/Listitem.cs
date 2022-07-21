using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class Listitem : MonoBehaviour
{
    [Tooltip("Кнопка входу в комнату")]
    [SerializeField] private Text _textName, _textPlayerCount;

    public void SetInfo(RoomInfo info)
    {
        // метод который даёт инфу о комнате на текст кнопки
        _textName.text = info.Name;
        _textPlayerCount.text = info.PlayerCount + "/" + info.MaxPlayers;

    }
    public void JoinRoom()
    {
        // присоеденение к комнате по кнопке
        PhotonNetwork.JoinRoom(_textName.text);
    }

}
   
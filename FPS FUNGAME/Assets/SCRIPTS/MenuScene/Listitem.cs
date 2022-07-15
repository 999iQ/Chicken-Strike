using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class Listitem : MonoBehaviour
{
    [Tooltip("Кнопка входу в комнату")]
    [SerializeField] private Text textName, textPlayerCount;

    public void SetInfo(RoomInfo info)
    {
        // метод который даёт инфу о комнате на текст кнопки
        textName.text = info.Name;
        textPlayerCount.text = info.PlayerCount + "/" + info.MaxPlayers;

    }
    public void JoinRoom()
    {
        // присоеденение к комнате по кнопке
        PhotonNetwork.JoinRoom(textName.text);
    }

}
   
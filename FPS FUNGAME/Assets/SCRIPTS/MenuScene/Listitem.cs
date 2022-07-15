using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class Listitem : MonoBehaviour
{
    [Tooltip("������ ����� � �������")]
    [SerializeField] private Text textName, textPlayerCount;

    public void SetInfo(RoomInfo info)
    {
        // ����� ������� ��� ���� � ������� �� ����� ������
        textName.text = info.Name;
        textPlayerCount.text = info.PlayerCount + "/" + info.MaxPlayers;

    }
    public void JoinRoom()
    {
        // ������������� � ������� �� ������
        PhotonNetwork.JoinRoom(textName.text);
    }

}
   
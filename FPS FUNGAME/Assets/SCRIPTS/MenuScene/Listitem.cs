using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class Listitem : MonoBehaviour
{
    [Tooltip("������ ����� � �������")]
    [SerializeField] private Text _textName, _textPlayerCount;

    public void SetInfo(RoomInfo info)
    {
        // ����� ������� ��� ���� � ������� �� ����� ������
        _textName.text = info.Name;
        _textPlayerCount.text = info.PlayerCount + "/" + info.MaxPlayers;

    }
    public void JoinRoom()
    {
        // ������������� � ������� �� ������
        PhotonNetwork.JoinRoom(_textName.text);
    }

}
   
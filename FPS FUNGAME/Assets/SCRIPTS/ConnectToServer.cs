using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    
    void Awake()
    {
        // ��������� ����� ����������� � ������ (��������� �����������)
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() // ������������ � �������
    {
        SceneManager.LoadScene("Menu");
        PhotonNetwork.JoinLobby(); // ����������� � ����� ����������� ����� ������ �������
    }

    public override void OnDisconnected(DisconnectCause cause) 
    {
        // �� ������������ � ������� (��� ���������)
        Debug.LogWarningFormat("ERROR - Disconnected to server", cause);
    }


}

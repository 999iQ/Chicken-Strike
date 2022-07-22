using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        // подключает к серверу
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() // обратный вызов когда подлключились
    {
        SceneManager.LoadScene("Menu");
        PhotonNetwork.JoinLobby(); // вступаем в дефолт лобби важно*
    }

    public override void OnDisconnected(DisconnectCause cause) 
    {
        // отключились от сервера (например кик)
        Debug.LogWarningFormat("ERROR - Disconnected to server", cause);
    }


}

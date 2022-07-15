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
        // отправная точка подключения к облаку (ЗАПУСКАЕТ ПОДКЛЮЧЕНИЕ)
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() // подключились к серверу
    {
        SceneManager.LoadScene("Menu");
        PhotonNetwork.JoinLobby(); // ПОДКЛЮЧЕНИЕ К ЛОББИ ОБЯЗАТЕЛЬНО ЧТОБЫ ВИДЕТЬ КОМНАТЫ
    }

    public override void OnDisconnected(DisconnectCause cause) 
    {
        // не подключились к серверу (нет интернета)
        Debug.LogWarningFormat("ERROR - Disconnected to server", cause);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        ValidateConnection(); // проверка соеденени€

        Application.targetFrameRate = 60;

        // ≈сли запустили игру до подключени€ к серверу
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("LoadingScene");
            return;
        }
    }
    public override void OnLeftRoom()
    {
        //вызываетс€ когда локальный игрок покидает комнату важно* тут сцену загружать
        SceneManager.LoadScene(0);
    }
    public void Leave()
    {
        // когда клиент (мы) вышли сами по кнопке
        PhotonNetwork.LeaveRoom();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // когда кто-то зашел вывод сообщени€
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // когда кто-то вышел вывод сообщени€
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
    }

    private void ValidateConnection()
    {
        // проверка подключени€ 
        if (PhotonNetwork.IsConnected) return;
        SceneManager.LoadScene(0);
    }


}

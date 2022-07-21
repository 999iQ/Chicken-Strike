using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class GameManager : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        ValidateConnection(); // проверка соеденения

        // Если запустили игру до подключения к серверу
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("LoadingScene");
            return;
        }
    }
    public override void OnLeftRoom()
    {
        //вызывается когда локальный игрок покидает комнату важно* тут сцену загружать
        SceneManager.LoadScene(0);
    }
    public void Leave()
    {
        // когда клиент (мы) вышли сами по кнопке
        PhotonNetwork.LeaveRoom();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // когда кто-то зашел вывод сообщения У ВСЕХ КРОМЕ ХОСТА
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);

        Hashtable PlayerCustomProps = new Hashtable();
        PlayerCustomProps["Kills"] = 0;
        PlayerCustomProps["Deaths"] = 0;
        // сохраняем и оповещаем остальных о настройках
        newPlayer.SetCustomProperties(PlayerCustomProps);

        Debug.Log("СРАБОТАЛ ОН ЕНТЕРЕД ЗЕ РУУМ!!!!!!!!!!!");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // когда кто-то вышел вывод сообщения
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
    }

    private void ValidateConnection()
    {
        // проверка подключения 
        if (PhotonNetwork.IsConnected) return;
        SceneManager.LoadScene(0);
    }


}

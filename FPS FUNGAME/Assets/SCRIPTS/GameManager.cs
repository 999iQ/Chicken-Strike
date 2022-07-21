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
        ValidateConnection(); // �������� ����������

        // ���� ��������� ���� �� ����������� � �������
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("LoadingScene");
            return;
        }
    }
    public override void OnLeftRoom()
    {
        //���������� ����� ��������� ����� �������� ������� �����* ��� ����� ���������
        SceneManager.LoadScene(0);
    }
    public void Leave()
    {
        // ����� ������ (��) ����� ���� �� ������
        PhotonNetwork.LeaveRoom();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // ����� ���-�� ����� ����� ��������� � ���� ����� �����
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);

        Hashtable PlayerCustomProps = new Hashtable();
        PlayerCustomProps["Kills"] = 0;
        PlayerCustomProps["Deaths"] = 0;
        // ��������� � ��������� ��������� � ����������
        newPlayer.SetCustomProperties(PlayerCustomProps);

        Debug.Log("�������� �� ������� �� ����!!!!!!!!!!!");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // ����� ���-�� ����� ����� ���������
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
    }

    private void ValidateConnection()
    {
        // �������� ����������� 
        if (PhotonNetwork.IsConnected) return;
        SceneManager.LoadScene(0);
    }


}

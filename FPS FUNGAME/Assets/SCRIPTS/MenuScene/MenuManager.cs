using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Listitem itemPrefab;
    [SerializeField] Transform roomListContent;
    public byte maxPlayersPerRoom = 8; // ���-�� ������� � �������

    public InputField NameRoom;
    public InputField PlayerName; // ��� ������
    const string playerNamePrefKey = "PlayerName"; // ���� ���������� ����

    public Text LogText;

    [SerializeField] string txt; // ��� ������� �����������
    public Text txtDebug;

    private void Awake()
    {
        // ������������� ���� � ������� ����� � ������� ����� (loadlevel)
        //PhotonNetwork.AutomaticallySyncScene = true;
        // ������ ����
        PhotonNetwork.GameVersion = "1";

    }
    private void Start() 
    {

        // ���� ��� � ����������
        string defaultName = string.Empty; // ������ �������
        if (PlayerName != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                PlayerName.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;
        Log("Player's name is set to " + PhotonNetwork.NickName);

    }
    void Update()
    {
        // ������� ����������� � ����� � ��������

        try
        {
            if (PhotonNetwork.OfflineMode) { txt = "OFFLINE"; return; }
            if (PhotonNetwork.IsConnectedAndReady)
            {
                txt = "Server= " + PhotonNetwork.ServerAddress + " | Lobby='" + PhotonNetwork.CurrentLobby?.Name + "' (" + PhotonNetwork.CurrentLobby?.Type + ") |" + PhotonNetwork.CloudRegion + "\nMaster=" + PhotonNetwork.CountOfPlayersOnMaster + " |Rooms=" + PhotonNetwork.CountOfRooms + " |Players In Rooms=" + PhotonNetwork.CountOfPlayersInRooms;
                if (PhotonNetwork.InRoom) { txt += "\nMyRoom=" + PhotonNetwork.CurrentRoom.PlayerCount + " [" + PhotonNetwork.CurrentRoom.IsOpen + "/" + PhotonNetwork.CurrentRoom.IsVisible + "] |State=" + PhotonNetwork.CurrentRoom.LoadBalancingClient?.State + "|Client=" + PhotonNetwork.CurrentRoom.LoadBalancingClient?.ClientType + "|Version=" + PhotonNetwork.CurrentRoom.LoadBalancingClient?.AppVersion; }
                txt += "\nVERSION PUN = " + PhotonNetwork.PunVersion;
            }
            else { txt = "NOT CONNECTED O.o"; }
        }
        catch (System.Exception ex) { txt = "(loading)... " + ex.Message; }
        txtDebug.text = txt;
    }

    public void SetPlayerName(string value) // ���������� ����
    {
        string _value = value;
        // #Important
        if (string.IsNullOrEmpty(value) || _value.Replace(" ","") == "") // ���� ���� ��� ��� ��� �������
        {
            _value = "Player" + Random.Range(1, 999);
        }

        _value = _value.Trim(); // ����� ������� ������� �� ������ � ����� ������

        // ��������� ��� �� �������
        PhotonNetwork.NickName = _value;

        // ��������� ��� �� �����
        PlayerPrefs.SetString(playerNamePrefKey, _value);
    }

    
    
    private void Log(string message) 
    {
        // ����� �������� � ��� �� �����

        Debug.Log(message);
        LogText.text += "\n";
        LogText.text += message;
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = maxPlayersPerRoom };

        // �������� ������� � ������������ ���-��� �������
        PhotonNetwork.CreateRoom(NameRoom.text, roomOptions);
    }

    public void JoinRoom()
    {
        // ����� ��� ������������� � ������� � ������ ( ����� �� ������ ����� ���� ����� )
        PhotonNetwork.JoinRoom(NameRoom.text);

    }


    public override void OnJoinedRoom() 
    {
        // ���������� ����� ����������� � ������� �� �������--
        // ������ ��������� ������
        Hashtable PlayerCustomProps = new Hashtable();
        PlayerCustomProps["Kills"] = 0;
        PlayerCustomProps["Deaths"] = 0;
        // ��������� � ��������� ��������� � ����������
        PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerCustomProps);
        Debug.Log("�������� �� ������� ����!!!!!!!!!!!");


        // �������� ����� (�����)
        Log("Joined the room");
        PhotonNetwork.LoadLevel("Map_" + Random.Range(1,3)); // ������ �� 1 �� ���-�� ���� + 1
        PhotonNetwork.NickName = "" + PlayerName;

    }
    public override void OnLeftRoom()
    {
        // ����� ������ (��) ����� �� �����-�� �������
        PhotonNetwork.LoadLevel("LoadingScene");
    }




    public override void OnConnectedToMaster()
    {
        Log("Connected to Master");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // ���������� ����� ����������� ����� �������

        foreach(Transform trans in roomListContent)
        {
            // ������ ���� ������ � ����� ���������
            Destroy(trans.gameObject);
        }

        for(int i = 0; i < roomList.Count; ++i)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(itemPrefab, roomListContent).GetComponent<Listitem>().SetInfo(roomList[i]);
        }

    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // �� ��������� �������
        Log("ERROR create room! Write to admin");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Listitem _itemPrefab;
    [SerializeField] private Transform _roomListContent;
    public byte maxPlayersPerRoom = 8; 

    public InputField NameRoom;
    public InputField PlayerName; 
    private const string playerNamePrefKey = "PlayerName"; // для удобства, чтобы не ошибиться

    public Text LogText;

    [SerializeField] private string _txt; // текст дебага
    public Text txtDebug;

    private void Awake()
    {
        // нужная вещь но вызывает неудобства, без неё можно зайти в 1 комнату на разные карты))
        PhotonNetwork.AutomaticallySyncScene = true;
        // для обновлений
        PhotonNetwork.GameVersion = "1";

    }

    private void Start() 
    {
        // подстановка сохранённого ника игрока
        string defaultName = string.Empty; 
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
    private void Update()
    {
        // дебаг о подключении к серверу

        try
        {
            if (PhotonNetwork.OfflineMode) { _txt = "OFFLINE"; return; }
            if (PhotonNetwork.IsConnectedAndReady)
            {
                _txt = "Server= " + PhotonNetwork.ServerAddress + " | Lobby='" + PhotonNetwork.CurrentLobby?.Name + "' (" + PhotonNetwork.CurrentLobby?.Type + ") |" + PhotonNetwork.CloudRegion + "\nMaster=" + PhotonNetwork.CountOfPlayersOnMaster + " |Rooms=" + PhotonNetwork.CountOfRooms + " |Players In Rooms=" + PhotonNetwork.CountOfPlayersInRooms;
                if (PhotonNetwork.InRoom) { _txt += "\nMyRoom=" + PhotonNetwork.CurrentRoom.PlayerCount + " [" + PhotonNetwork.CurrentRoom.IsOpen + "/" + PhotonNetwork.CurrentRoom.IsVisible + "] |State=" + PhotonNetwork.CurrentRoom.LoadBalancingClient?.State + "|Client=" + PhotonNetwork.CurrentRoom.LoadBalancingClient?.ClientType + "|Version=" + PhotonNetwork.CurrentRoom.LoadBalancingClient?.AppVersion; }
                _txt += "\nVERSION PUN = " + PhotonNetwork.PunVersion;
            }
            else { _txt = "NOT CONNECTED O.o"; }
        }
        catch (System.Exception ex) { _txt = "(loading)... " + ex.Message; }
        txtDebug.text = _txt;
    }

    public void SetPlayerName(string value) // запоминание ника
    {
        string _value = value;
        // #Important
        if (string.IsNullOrEmpty(value) || _value.Replace(" ","") == "") // ограничения для ника
        {
            _value = "Player" + Random.Range(1, 999);
        }

        _value = _value.Trim(); // убирает пробелы с начала и конца ника

        PhotonNetwork.NickName = _value;

        PlayerPrefs.SetString(playerNamePrefKey, _value);
    }

    
    
    private void Log(string message) 
    {
        // типо удобный дебаг лог
        Debug.Log(message);
        LogText.text += "\n";
        LogText.text += message;
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = maxPlayersPerRoom };

        // создание комнаты с опциями
        PhotonNetwork.CreateRoom(NameRoom.text, roomOptions);
    }

    public void JoinRoom()
    {
        // функция входа в комнату для кнопки
        PhotonNetwork.JoinRoom(NameRoom.text);
    }


    public override void OnJoinedRoom() 
    {
        // обратный вызов после вхождения в комнату

        Hashtable PlayerCustomProps = new Hashtable();
        PlayerCustomProps["Kills"] = 0;
        PlayerCustomProps["Deaths"] = 0;
        // создали статистику игрока
        PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerCustomProps);
        
        Debug.Log("�������� �� ������� ����!");


        Log("Joined the room");
        PhotonNetwork.LoadLevel("Map_" + Random.Range(1,3)); // колво карт + 1
        PhotonNetwork.NickName = "" + PlayerName;

    }
    public override void OnLeftRoom()
    {
        // если вышли из комнаты запускается загрузка
        PhotonNetwork.LoadLevel("LoadingScene");
    }


    public override void OnConnectedToMaster()
    {
        // подключение к серверу
        Log("Connected to Master");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // обновление списка комнат

        foreach(Transform trans in _roomListContent)
        {
            // удаляем все и снова спавним все
            Destroy(trans.gameObject);
        }

        for(int i = 0; i < roomList.Count; ++i)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(_itemPrefab, _roomListContent).GetComponent<Listitem>().SetInfo(roomList[i]);
        }

    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // если не получилось создать комнату, обычно когда такое название уже есть
        Log("ERROR create room! Write to admin");
    }

}

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
    public byte maxPlayersPerRoom = 8; // кол-во игроков в комнате

    public InputField NameRoom;
    public InputField PlayerName; // ник игрока
    private const string playerNamePrefKey = "PlayerName"; // ключ сохранения ника

    public Text LogText;

    [SerializeField] private string _txt; // для отладки подключения
    public Text txtDebug;

    private void Awake()
    {
        // синхронизация сцен и левелов юнити и сервера фотон (loadlevel) ИЗ-ЗА НЕЁ НЕ РАБОТАЮТ ОБРАТНЫЕ ВОЗОВЫ КОМНАТЫ
        PhotonNetwork.AutomaticallySyncScene = true;
        // версия игры
        PhotonNetwork.GameVersion = "1";

    }

    private void Start() 
    {

        // ищет ник в сохранёнках
        string defaultName = string.Empty; // пустая строчка
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
        // отладка подключения к лобби и комнатам

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

    public void SetPlayerName(string value) // сохранение ника
    {
        string _value = value;
        // #Important
        if (string.IsNullOrEmpty(value) || _value.Replace(" ","") == "") // если ника нет или это пробелы
        {
            _value = "Player" + Random.Range(1, 999);
        }

        _value = _value.Trim(); // метод убирает пробелы из начала и конца строки

        // сохраняем ник на сервере
        PhotonNetwork.NickName = _value;

        // сохраняем ник на компе
        PlayerPrefs.SetString(playerNamePrefKey, _value);
    }

    
    
    private void Log(string message) 
    {
        // вывод действий в лог на экран

        Debug.Log(message);
        LogText.text += "\n";
        LogText.text += message;
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = maxPlayersPerRoom };

        // создание комнаты с определенным кол-вом игроков
        PhotonNetwork.CreateRoom(NameRoom.text, roomOptions);
    }

    public void JoinRoom()
    {
        // метод для подсоеденения к комнате с именем ( также на кнопке висит этот метод )
        PhotonNetwork.JoinRoom(NameRoom.text);

    }


    public override void OnJoinedRoom() 
    {
        // вызывается после подключения к комнате на сервере-- У ВСЕХ КРОМЕ ХОСТА
        // создаём настройки игрока

        Hashtable PlayerCustomProps = new Hashtable();
        PlayerCustomProps["Kills"] = 0;
        PlayerCustomProps["Deaths"] = 0;
        // сохраняем и оповещаем остальных о настройках
        PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerCustomProps);
        
        Debug.Log("СРАБОТАЛ ОН ДЖОИНЕД РУУМ!");


        // ЗАГРУЗКА КАРТЫ (СЦЕНЫ)
        Log("Joined the room");
        PhotonNetwork.LoadLevel("Map_" + Random.Range(1,3)); // РАНДОМ ОТ 1 ДО КОЛ-ВА КАРТ + 1
        PhotonNetwork.NickName = "" + PlayerName;

    }
    public override void OnLeftRoom()
    {
        // когда клиент (мы) вышли по какой-то причине
        PhotonNetwork.LoadLevel("LoadingScene");
    }


    public override void OnConnectedToMaster()
    {
        Log("Connected to Master");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // вызывается когда добавляются новые комнаты

        foreach(Transform trans in _roomListContent)
        {
            // чистим лист комнат а потом заполняем
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
        // не создалась комната
        Log("ERROR create room! Write to admin");
    }

}

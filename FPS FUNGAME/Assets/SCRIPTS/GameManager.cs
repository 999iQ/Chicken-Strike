using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class GameManager : MonoBehaviourPunCallbacks
{
    private ScoreBoard scoreBoard; // ссылка на скорборд для создания плашек статистики игроков

    [Header("Область переменных таймера матча")]
    [SerializeField] private double _incTimer;
    [SerializeField] private double _startTime; // время фотона когда хост зашел в комнату
    private TMP_Text _textMeshTimer;
    public double timer = 300; // время матча

    private void Awake()
    {
        ValidateConnection(); // проверка подключения
        scoreBoard = FindObjectOfType<ScoreBoard>();

        _textMeshTimer = GameObject.Find("TIMER").GetComponent<TMP_Text>();
    }
    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            // Хост сохраняет глобальное время сервера как стартовое в настройки команты
            Hashtable RoomCustomProps = new Hashtable();
            RoomCustomProps["StartTime"] = PhotonNetwork.Time;
            _startTime = PhotonNetwork.Time;
            PhotonNetwork.CurrentRoom.SetCustomProperties(RoomCustomProps);
        }
        else
        {
            // Если мы не хост, то получаем время от комнаты
            _startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["StartTime"];
        }
    }
    private void Update()
    {
#region Таймер матча

        _incTimer = PhotonNetwork.Time - _startTime;
        string minutes = ( (int)(timer - _incTimer) / 60 ).ToString("00");
        string seconds = ( (int)(timer - _incTimer) % 60 ).ToString("00");
        
        _textMeshTimer.text = $"{minutes} : {seconds}";

        if (_incTimer >= timer) //время всегда увеличивается | когда разница > время матча -> метод
        {
            EndMatch();
        }
#endregion

    }
    public void EndMatch() // конец матча 
    {
        Invoke(nameof(Leave), 5f);
    }

    public void Leave()
    {
        // метод для кнопки выхода из матча
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinedRoom()
    {

        Debug.Log("(ГЕЙММЕНЕДЖЕР) СРАБОТАЛ ОН джоинед РУМ!");
    }
    public override void OnLeftRoom()
    {

        //когда вышли из комнаты запускаем сцену загрузки
        SceneManager.LoadScene(0);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);

        Hashtable PlayerCustomProps = new Hashtable();
        PlayerCustomProps["Kills"] = 0;
        PlayerCustomProps["Deaths"] = 0;
        // создаём статистику игрока, ДОДЕЛАТЬ 
        newPlayer.SetCustomProperties(PlayerCustomProps);

        scoreBoard.AddScoreboardItem(newPlayer); // даём ссылку на игрока в скорборд для плашки

        Debug.Log("(ГЕЙММЕНЕДЖЕР) СРАБОТАЛ ОН ЕНТЕРЕД РУМ!");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        scoreBoard.RemoveScoreboardItem(otherPlayer); // даём ссылку на игрока в скорборд для плашки

        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
    }

    private void ValidateConnection()
    {
        if (PhotonNetwork.IsConnected) return;
        SceneManager.LoadScene(0);
    }


}

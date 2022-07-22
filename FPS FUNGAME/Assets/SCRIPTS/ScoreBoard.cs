using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ScoreBoard : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform _container; // то где спавнятся плашки
    [SerializeField] private GameObject _scoreboardItemPrefab; // префаб плашки для спавна

    private void Start()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
        }
    }

    // словарь с игроками на пару с их плашкой (для удобного доступа и связи между ними
    private Dictionary<Player, ScoreBoardItem> scoreboardItems_Dic = new Dictionary<Player, ScoreBoardItem>();

    private void AddScoreboardItem(Player player)
    {
        //спавним плашку со статой о игроке
        ScoreBoardItem item = Instantiate(_scoreboardItemPrefab, _container).GetComponent<ScoreBoardItem>();

        item.Initialize(player);

        scoreboardItems_Dic[player] = item; // добовляем его в словарь
    }

    private void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems_Dic[player].gameObject); // удаляет плашку когда игрок выходит
        scoreboardItems_Dic.Remove(player); // удаляет его из словаря
    }


    //методы срабатывают когда игроки входят и выходят
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }
}

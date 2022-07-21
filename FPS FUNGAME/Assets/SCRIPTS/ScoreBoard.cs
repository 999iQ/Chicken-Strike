using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ScoreBoard : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform _container; // то куда мы спавним
    [SerializeField] private GameObject _scoreboardItemPrefab; // то что мы спавним

    private void Start()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            // когда любой игрок заходит на сцену он добавл€ет себ€ в список
            AddScoreboardItem(player);
        }
    }

    // cловарь дл€ сохранени€ ссылки на окошко в табло по имени игрока
    private Dictionary<Player, ScoreBoardItem> scoreboardItems_Dic = new Dictionary<Player, ScoreBoardItem>();

    private void AddScoreboardItem(Player player)
    {
        //—ќ«ƒј®“—я —¬ќя ѕЋјЎ ј
        ScoreBoardItem item = Instantiate(_scoreboardItemPrefab, _container).GetComponent<ScoreBoardItem>();

        item.Initialize(player);

        scoreboardItems_Dic[player] = item; // добавили элемент (игрока) в словарь
    }

    private void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems_Dic[player].gameObject); // удаление плашки с игроком
        scoreboardItems_Dic.Remove(player); // удаление из словар€ (из масива)
    }


    //методы которые сами вызываютс€ при входе и выходе игрока ” ¬—≈’  –ќћ≈ ’ќ—“ј Ѕјјјјјјјјјјјјјјјјјјј√√√√
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }
}

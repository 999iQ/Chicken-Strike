using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ScoreBoard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container; // то куда мы спавним
    [SerializeField] GameObject scoreboardItemPrefab; // то что мы спавним

    private void Start()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            // когда любой игрок заходит на сцену он добавляет себя в список
            AddScoreboardItem(player);
        }
    }

    // cловарь для сохранения ссылки на окошко в табло по имени игрока
    Dictionary<Player, ScoreBoardItem> scoreboardItems = new Dictionary<Player, ScoreBoardItem>();

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }

    void AddScoreboardItem(Player player)
    {
        ScoreBoardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreBoardItem>();
        item.Initialize(player);
        scoreboardItems[player] = item; // добавили элемент (игрока) в словарь
    }

    void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems[player].gameObject); // удаление плашки с игроком
        scoreboardItems.Remove(player); // удаление из словаря (из масива)
    }
}

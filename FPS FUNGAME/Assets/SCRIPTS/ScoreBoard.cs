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
    Dictionary<Player, ScoreBoardItem> scoreboardItems_Dic = new Dictionary<Player, ScoreBoardItem>();



    void AddScoreboardItem(Player player)
    {
        ScoreBoardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreBoardItem>();
        item.Initialize(player);
        scoreboardItems_Dic[player] = item; // добавили элемент (игрока) в словарь
    }

    void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems_Dic[player].gameObject); // удаление плашки с игроком
        scoreboardItems_Dic.Remove(player); // удаление из словаря (из масива)
    }


    //методы которые сами вызываются при входе и выходе игрока
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;

public class ScoreBoard : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        // на старте все плашки прозрачные
        foreach (var item in GetComponentsInChildren<CanvasGroup>())
            item.alpha = 0;
    }

    public void UpdateScoreBoard(List<Player> playerList)
    {
        // сортируем игроков по убийствам
        Player[] top = playerList.OrderByDescending(p => (int)p.CustomProperties["Kills"]).ToArray();
        
        for(int i = 0; i < top.Length; ++i)
        {
            transform.GetChild(i).GetComponent<CanvasGroup>().alpha = 1;
            transform.GetChild(i).GetComponent<ScoreBoardItem>().usernameText.text = (i + 1)+ ". " + top[i].NickName;
            transform.GetChild(i).GetComponent<ScoreBoardItem>().killsText.text = ((int)top[i].CustomProperties["Kills"]).ToString();
            transform.GetChild(i).GetComponent<ScoreBoardItem>().deathsText.text = ((int)top[i].CustomProperties["Deaths"]).ToString();
        }
    }
}

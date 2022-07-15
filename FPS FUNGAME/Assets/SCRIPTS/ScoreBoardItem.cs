using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;


public class ScoreBoardItem : MonoBehaviourPunCallbacks
{
    // таблички для каждого игрока: убийств и смертей игроков-
    public TMP_Text usernameText, killsText, deathsText;
    public Player _player;

    public void Initialize(Player player) // отслеживание всего, что связано с игроком
    {
        transform.name = player.NickName + "_Score";
        usernameText.text = player.NickName;

        //transform.name = PlayerPrefs.GetString("PlayerName");
        //usernameText.text = PlayerPrefs.GetString("PlayerName");


        _player = player;
        
        
    }
    private void Update()
    {
        // это можно оптимизировать методом onPlayerCustomPRop..
        usernameText.text = _player.NickName;
        killsText.text = _player.CustomProperties.ContainsKey("Kills") ? _player.CustomProperties["Kills"].ToString() : "0";
        deathsText.text = _player.CustomProperties.ContainsKey("Deaths") ? _player.CustomProperties["Deaths"].ToString() : "0";

    }


}

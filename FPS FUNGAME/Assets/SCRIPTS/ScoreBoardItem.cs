using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class ScoreBoardItem : MonoBehaviourPunCallbacks
{
    // плашка со статой на ТАБ
    public TMP_Text usernameText, killsText, deathsText;
    private Player _player;

    //ЕСТЬ БАГИ ДОДЕЛАТЬ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public void Initialize(Player player) // срабатывает когда заходит новый игрок
    {
        transform.name = player.NickName + "_Score";
        usernameText.text = player.NickName;

        _player = player;

        UpdateStats();
    }
    private void Start()
    {
        usernameText.text = _player.NickName;
    }
    private void UpdateStats()
    {
        killsText.text = _player.CustomProperties.ContainsKey("Kills") ? _player.CustomProperties["Kills"].ToString() : "0";
        deathsText.text = _player.CustomProperties.ContainsKey("Deaths") ? _player.CustomProperties["Deaths"].ToString() : "0";
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(targetPlayer == _player)
        {
            if(changedProps.ContainsKey("Kills") || changedProps.ContainsKey("Deaths"))
            {
                UpdateStats();
            }
        }
    }


}

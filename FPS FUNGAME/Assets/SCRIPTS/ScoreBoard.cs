using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ScoreBoard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container; // �� ���� �� �������
    [SerializeField] GameObject scoreboardItemPrefab; // �� ��� �� �������

    private void Start()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            // ����� ����� ����� ������� �� ����� �� ��������� ���� � ������
            AddScoreboardItem(player);
        }
    }

    // c������ ��� ���������� ������ �� ������ � ����� �� ����� ������
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
        scoreboardItems[player] = item; // �������� ������� (������) � �������
    }

    void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems[player].gameObject); // �������� ������ � �������
        scoreboardItems.Remove(player); // �������� �� ������� (�� ������)
    }
}

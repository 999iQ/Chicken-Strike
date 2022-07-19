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
    Dictionary<Player, ScoreBoardItem> scoreboardItems_Dic = new Dictionary<Player, ScoreBoardItem>();

    void AddScoreboardItem(Player player)
    {
        //��������� ���� ������
        ScoreBoardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreBoardItem>();

        item.Initialize(player);

        scoreboardItems_Dic[player] = item; // �������� ������� (������) � �������
    }

    void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems_Dic[player].gameObject); // �������� ������ � �������
        scoreboardItems_Dic.Remove(player); // �������� �� ������� (�� ������)
    }


    //������ ������� ���� ���������� ��� ����� � ������ ������ � ���� ����� ����� ������������������������
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }
}

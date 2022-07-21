using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    public GameObject player;
    public Transform[] spawners;

    private void Start()
    {
        SpawnPlayers();
    }
    public void SpawnPlayers()
    {
        
        int i = Random.Range(0, spawners.Length);
        PhotonNetwork.Instantiate(player.name, spawners[i].position, Quaternion.identity);
        
        
    }


}

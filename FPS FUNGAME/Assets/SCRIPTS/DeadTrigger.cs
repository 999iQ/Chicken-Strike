using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeadTrigger : MonoBehaviourPunCallbacks
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") 
            other.GetComponent<PlayerContr>().TakeDamage(1000, string.Empty);
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeadTrigger : MonoBehaviourPunCallbacks
{
    private void OnTriggerEnter(Collider other)
    {
        // зона смерти в пустоте
        if (other.gameObject.tag == "Player") 
            other.GetComponent<PlayerContr>().TakeDamage(1000, string.Empty);
        
    }
}

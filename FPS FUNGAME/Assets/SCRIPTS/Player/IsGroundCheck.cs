using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsGroundCheck : MonoBehaviour
{
    public bool IsGround = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            IsGround = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            IsGround = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            IsGround = false;
        }
    }
}

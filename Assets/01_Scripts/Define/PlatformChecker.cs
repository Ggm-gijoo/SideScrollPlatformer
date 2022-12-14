using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformChecker : MonoBehaviour
{
    Collider thisColl;
    private bool isTriggered = false;
    private GameObject player;
    private Animator playerAnim;
    private PlayerController playerMove;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerAnim = player.GetComponent<Animator>();
        playerMove = player.GetComponentInParent<PlayerController>();
        thisColl = this.GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlatformCheck") && !isTriggered)
        {
            isTriggered = true;
            thisColl.isTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("PlatformCheck"))
        {
            isTriggered = false;
            thisColl.isTrigger = false;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerMove.IsLand = false;
            playerAnim.SetInteger("TriggerNumber", (int)AnimState.Jump);
            playerMove.jumpCount = 2;
            playerAnim.SetTrigger("Trigger");
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            if(Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow))
            {
                isTriggered = true;
                thisColl.isTrigger = true;
                playerMove.jumpCount = 2;
                playerMove.IsLand = false;
                playerAnim.SetInteger("TriggerNumber", (int)AnimState.Jump);
                playerAnim.SetTrigger("Trigger");
            }
        }
    }
}

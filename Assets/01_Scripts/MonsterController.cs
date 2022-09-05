using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("weapon"))
        {
            Debug.Log("¸Â¾Æ");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private float hp;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("weapon"))
        {
        }
    }
}

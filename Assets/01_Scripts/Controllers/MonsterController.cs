using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    HPModule hpModule;

    private void Awake()
    {
        hpModule = GetComponent<HPModule>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("weapon"))
        {
            hpModule.Damage(other.GetComponent<WeaponDefault>().damage);
        }
        else if(other.CompareTag("Player"))
        {

        }
    }
}

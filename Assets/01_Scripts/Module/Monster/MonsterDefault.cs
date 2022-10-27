using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterDefault : MonoBehaviour
{
    HPModule hpModule;

    private void Awake()
    {
        hpModule = GetComponent<HPModule>();
        if (hpModule == null)
            hpModule = gameObject.AddComponent<HPModule>();
    }
    public abstract void Attack();
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("weapon"))
        {
            hpModule.Damage(other.GetComponent<WeaponDefault>().damage);
        }
    }
}

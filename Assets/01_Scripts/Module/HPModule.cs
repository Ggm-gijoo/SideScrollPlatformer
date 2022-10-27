using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPModule : MonoBehaviour
{
    [SerializeField] private float maxHp = 0;
    private float nowHp = 0;

    [HideInInspector]
    public float NowHp { get => nowHp; set => nowHp = value; }

    private void Awake()
    {
        nowHp = maxHp;
    }

    public void Damage(float damage)
    {
        nowHp = nowHp - damage < 0? 0 : nowHp - damage;
    }
    public void Heal(float heal)
    {
        nowHp = nowHp + heal > maxHp? maxHp : nowHp + heal;
    }
}

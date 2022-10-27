using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class W_00_None : WeaponDefault
{
    public override float damage { get; set; } = 5f;

    public override void Attack(int attackMove, Action<int> Callback = null)
    {
        Managers.Sound.Play($"Player/Sword_Swing_0{Random.Range(0, 2)}");
        if ((attackMove + 1) % 2 == 0)
        {
            Variables.Instance.AttackCollider[1].enabled = true;
        }
        else
        {
            Variables.Instance.AttackCollider[0].enabled = true;
        }
        Callback?.Invoke(0);
    }
    public override int ReturnAttackMove(int attackMove)
    {
        attackMove = attackMove % 6 + 1;
        return attackMove;
    }
}

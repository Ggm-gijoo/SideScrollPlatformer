using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_02_Bow : WeaponDefault
{
    float chargeTime;

    public override int Attack(int attackMove)
    {
        StartCoroutine(ChargeAttack());
        attackMove = attackMove % 3 + 1;
        return attackMove;
    }

    private IEnumerator ChargeAttack()
    {
        while (chargeTime <= 5f && Input.GetMouseButton(0))
        {
            yield return new WaitUntil(() => Input.GetMouseButtonUp(0));

        }
    }
}

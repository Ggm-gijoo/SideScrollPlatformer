using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_02_Bow : WeaponDefault
{
    public override void Attack(int attackMove, Action<int> Callback = null)
    {
        StartCoroutine(ChargeAttack(attackMove, Callback));
    }

    public override int ReturnAttackMove(int attackMove)
    {
        attackMove = attackMove % 3 + 1;
        return attackMove;
    }

    private IEnumerator ChargeAttack(int attackMove, Action<int> CallBack)
    {
        Debug.Log("ChargeAttack S");
        float chargeTime = 0f;

        while (chargeTime <= 5f && Input.GetMouseButton(0))
        {
            chargeTime += Time.deltaTime;
            yield return null;
        }

        if (chargeTime >= 5f)
            Debug.Log("Charged!");

        chargeTime = 0;
        
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        CallBack?.Invoke(attackMove);
        Debug.Log("ChargeAttack E");
    }

}

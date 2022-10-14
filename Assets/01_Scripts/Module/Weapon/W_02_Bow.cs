using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_02_Bow : WeaponDefault
{
    float chargeTime;

    public override int Attack(int attackMove)
    {
        StartCoroutine(ChargeAttack(attackMove));
        return attackMove;
    }

    private IEnumerator ChargeAttack(int attackMove)
    {
        while (chargeTime <= 5f && Input.GetMouseButton(0))
        {
            Debug.Log("Charging...");
            chargeTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Charged!");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        attackMove = attackMove % 3 + 1;
    }
}

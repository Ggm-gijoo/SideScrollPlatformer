using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_02_Bow : WeaponDefault
{
    [Header("차지 파티클")]
    [SerializeField]
    private ParticleSystem[] prepareParticles;
    [SerializeField]
    private ParticleSystem[] chargedParticles;

    private bool isCharged = false;

    public override void Attack(int attackMove, Action<int> Callback = null)
    {
        StartCoroutine(ChargeAttack(attackMove, Callback));
    }

    public override int ReturnAttackMove(int attackMove)
    {
        if (isCharged)
            attackMove = 2;
        else
            attackMove = 1;
        return attackMove;
    }

    private IEnumerator ChargeAttack(int attackMove, Action<int> CallBack)
    {
        Debug.Log("ChargeAttack S");

        Variables.Instance.PlayerAnim.SetInteger("Action", 0);

        Variables.Instance.PlayerAnim.SetTrigger("Trigger");

        float chargeTime = 0f;

        while (chargeTime <= 2f && Input.GetMouseButton(0))
        {
            chargeTime += Time.deltaTime;
            yield return null;
        }
        isCharged = false;
        if (chargeTime >= 2f)
        {
            Debug.Log("Charged!");
            isCharged = true;
        }

        chargeTime = 0;
        
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        CallBack?.Invoke(attackMove);
        Debug.Log("ChargeAttack E");
    }

}

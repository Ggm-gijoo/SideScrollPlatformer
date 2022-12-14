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

    public override float Damage { get; set; } = 10;

    public override void Attack(int attackMove, Action<int> Callback = null)
    {
        StartCoroutine(ChargeAttack(attackMove, Callback));
    }

    public override int ReturnAttackMove(int attackMove)
    {
        if (isCharged)
        {
            Damage = 20;
            attackMove = 2;
        }
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

        foreach (ParticleSystem part in prepareParticles)
            part.Play();
        while (chargeTime <= 1.5f && Input.GetKey(KeyCode.A))
        {
            chargeTime += Time.deltaTime;
            yield return null;
        }

        isCharged = false;
        foreach (ParticleSystem part in prepareParticles)
            part.Stop();

        if (chargeTime >= 1.5f)
        {
            Debug.Log("Charged!");
            isCharged = true;
            foreach (ParticleSystem part in chargedParticles)
                part.Play();
        }

        chargeTime = 0;
        
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.A));
        foreach (ParticleSystem part in chargedParticles)
            part.Stop();
        CallBack?.Invoke(attackMove);
        Debug.Log("ChargeAttack E");
    }

}

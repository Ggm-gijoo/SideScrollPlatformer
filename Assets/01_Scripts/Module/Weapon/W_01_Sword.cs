using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class W_01_Sword : WeaponDefault
{

    public override void Attack(int attackMove, Action<int> Callback = null)
    {
        StartCoroutine(WeaponVfxPlay(attackMove, new WaitForSeconds(0.3f)));
        Variables.Instance.AttackCollider[2].enabled = true;
        Callback?.Invoke(0);
    }
    public override int ReturnAttackMove(int attackMove)
    {
        attackMove = attackMove % 6 + 1;

        return attackMove;
    }

    public override void Skill()
    {
        Variables.Instance.WeaponSkillVfx[(int)WeaponState.Sword]?.SetActive(true);
        Time.timeScale = 0.7f;
    }
    public override void SkillEffect()
    {
        Time.timeScale = 1f;
        Variables.Instance.WeaponSkillVfx[(int)WeaponState.Sword]?.SetActive(false);
        Variables.Instance.WeaponSkillVfx[(int)WeaponState.Sword].transform.localPosition = Vector3.zero;
    }

    public IEnumerator WeaponVfxPlay(int attackMove, WaitForSeconds delayTime)
    {
        Variables.Instance.WeaponVfx[0].transform.localRotation = Quaternion.Euler(0, 0, attackMove % 2 * 180f);

        yield return delayTime;

        Variables.Instance.WeaponVfx[0].gameObject.SetActive(true);

        Managers.Sound.Play($"Player/Sword{Random.Range(1, 7)}");

        yield return delayTime;

        Variables.Instance.WeaponVfx[0].gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_01_Sword : WeaponDefault
{

    public override int Attack(int attackMove)
    {
        StartCoroutine(WeaponVfxPlay(attackMove, new WaitForSeconds(0.3f)));
        Variables.Instance.AttackCollider[2].enabled = true;
        
        attackMove = attackMove % 6 + 1;

        return attackMove;
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

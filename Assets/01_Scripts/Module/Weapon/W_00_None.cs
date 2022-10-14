using UnityEngine;

public class W_00_None : WeaponDefault
{
    public override int Attack(int attackMove)
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

        attackMove = attackMove  % 6 + 1;
        return attackMove;
    }
}

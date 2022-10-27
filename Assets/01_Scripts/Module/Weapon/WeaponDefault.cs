using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponDefault : MonoBehaviour
{
    public abstract float damage { get; set; }
    public abstract void Attack(int attackMove, Action<int> Callback = null);
    public abstract int ReturnAttackMove(int attackMove);
    public virtual void Skill() { }
    public virtual void SkillEffect() { }

}

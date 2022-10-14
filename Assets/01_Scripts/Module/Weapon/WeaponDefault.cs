using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponDefault : MonoBehaviour
{
    public abstract int Attack(int attackMove);
    public virtual void Skill() { }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="플레이어 스테이터스", menuName ="PlayerStatus")]
public class PlayerStatus : ScriptableObject
{
    [SerializeField]
    [Header("이동 속도")]
    private float moveSpd;
    [SerializeField]
    [Header("점프력")]
    [Space(3)]
    private float jumpForce;
    [SerializeField]
    [Header("체력")]
    private float hp;
    [SerializeField]
    [Header("마나")]
    private float mp;
    [SerializeField]
    [Header("기력")]
    private float stamina;

    public float MoveSpd { get { return moveSpd; } set { moveSpd = value; } }
    public float JumpForce { get { return jumpForce; } set { jumpForce = value; } }
    public float Hp { get { return hp; } set { hp = value; } }
    public float Mp { get { return mp; } set { mp = value; } }
    public float Stamina { get { return stamina; } set { stamina = value; } }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="�÷��̾� �������ͽ�", menuName ="PlayerStatus")]
public class PlayerStatus : ScriptableObject
{
    [SerializeField]
    [Header("�̵� �ӵ�")]
    private float moveSpd;
    [SerializeField]
    [Header("������")]
    [Space(3)]
    private float jumpForce;
    [SerializeField]
    [Header("ü��")]
    private float hp;
    [SerializeField]
    [Header("����")]
    private float mp;
    [SerializeField]
    [Header("���")]
    private float stamina;

    public float MoveSpd { get { return moveSpd; } set { moveSpd = value; } }
    public float JumpForce { get { return jumpForce; } set { jumpForce = value; } }
    public float Hp { get { return hp; } set { hp = value; } }
    public float Mp { get { return mp; } set { mp = value; } }
    public float Stamina { get { return stamina; } set { stamina = value; } }
}

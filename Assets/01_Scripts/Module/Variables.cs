using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variables : MonoBehaviour
{
    static Variables instance;

    public static Variables Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (instance == null)
                instance = new Variables();
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    [Space]
    [Header("���� ����")]
    [SerializeField]
    private Collider[] attackCollider;

    [Header("���� VFX")]
    [SerializeField]
    private GameObject[] weaponVfx;

    [Header("���� ��ų VFX")]
    [SerializeField]
    private GameObject[] weaponSkillVfx;

    [Header("�÷��̾� �ִϸ�����")]
    [SerializeField]
    private Animator playerAnim;

    public Collider[] AttackCollider => attackCollider;
    public GameObject[] WeaponVfx => weaponVfx;
    public GameObject[] WeaponSkillVfx => weaponSkillVfx;
    public Animator PlayerAnim => playerAnim;


}

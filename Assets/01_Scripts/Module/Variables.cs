using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
    private VisualEffect[] weaponVfx;

    [Header("���� ��ų VFX")]
    [SerializeField]
    private VisualEffect[] weaponSkillVfx;

    [Header("�÷��̾� �ִϸ�����")]
    [SerializeField]
    private Animator playerAnim;

    public Collider[] AttackCollider => attackCollider;
    public VisualEffect[] WeaponVfx => weaponVfx;
    public VisualEffect[] WeaponSkillVfx => weaponSkillVfx;
    public Animator PlayerAnim => playerAnim;


}

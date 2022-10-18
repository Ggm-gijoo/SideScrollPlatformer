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
    [Header("공격 판정")]
    [SerializeField]
    private Collider[] attackCollider;

    [Header("무기 VFX")]
    [SerializeField]
    private GameObject[] weaponVfx;

    [Header("무기 스킬 VFX")]
    [SerializeField]
    private GameObject[] weaponSkillVfx;

    [Header("플레이어 애니메이터")]
    [SerializeField]
    private Animator playerAnim;

    public Collider[] AttackCollider => attackCollider;
    public GameObject[] WeaponVfx => weaponVfx;
    public GameObject[] WeaponSkillVfx => weaponSkillVfx;
    public Animator PlayerAnim => playerAnim;


}

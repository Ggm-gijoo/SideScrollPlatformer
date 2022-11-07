using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 SO")]
    [SerializeField] private PlayerStatus playerStatus;

    [Header("무기")]
    [SerializeField] GameObject[] weapons;

    [SerializeField][ColorUsage(true, true, 1,1,1,1)] Color whiteColor;
    ChararcterTrail chararcterTrail;
    Renderer[][] weaponRenderer = new Renderer[100][];
    List<List<Renderer>> weaponRenderers = new List<List<Renderer>>();
    Dictionary<int, Renderer[]> weaponRenders = new Dictionary<int, Renderer[]>();
    
    private Rigidbody playerRigid;
    #region 스테이터스
    [HideInInspector] public float playerNowHp;
    [HideInInspector] public float playerNowMp;
    [HideInInspector] public float playerNowStamina;

    [HideInInspector] public int jumpCount = 0;
    private static int attackMove = 0;
    private int weaponStateValue = 0;

    private bool isAct = false;
    private bool isAttack = false;
    private bool isDodge = false;

    public bool IsLand = false;
    public bool IsSkill = false;

    #region 메모리캐싱
    private const string _alpha = "_Alpha";

    private readonly int _trigger = Animator.StringToHash("Trigger");
    private readonly int _triggerNum = Animator.StringToHash("TriggerNumber");
    private readonly int _moving = Animator.StringToHash("Moving");
    private readonly int _weapon = Animator.StringToHash("Weapon");
    private readonly int _action = Animator.StringToHash("Action");
    private readonly int _jumping = Animator.StringToHash("Jumping");
    private readonly int _skill = Animator.StringToHash("Skill");
    #endregion
    WeaponState weaponState = WeaponState.None;
    WeaponType weaponType = WeaponType.Light;
    #endregion

    private void Start()
    {
        playerRigid = GetComponent<Rigidbody>();
        chararcterTrail = GetComponent<ChararcterTrail>();

        int i = 0;
        foreach(var weapon in weapons)
        {
            weaponRenderer[i] = weapon.GetComponentsInChildren<MeshRenderer>();
            weaponRenders.Add(i, weapon.GetComponentsInChildren<MeshRenderer>());
            weapon.SetActive(false);
            i++;
        }
        Variables.Instance.WeaponVfx[0].gameObject.SetActive(true);

        foreach (var attColl in Variables.Instance.AttackCollider)
        {
            attColl.enabled = false;
        }

        playerNowHp = playerStatus.Hp;
        playerNowMp = playerStatus.Mp;
        playerNowStamina = playerStatus.Stamina;

        StartCoroutine(RecoveryStamina());
    }

    private void Update()
    {
        Variables.Instance.PlayerAnim.transform.position = ClampPos(Variables.Instance.PlayerAnim.transform.position);
        if (!isAttack && !isDodge && !IsSkill && Time.timeScale == 1)
        {
            Move();
            Jump();
            Dodge();
            Skill();
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                switch (weaponState)
                {
                    case WeaponState.None:
                        StartCoroutine(SummonWeapon());
                        weaponType = WeaponType.Heavy;
                        break;
                    case WeaponState.Sword:
                        weapons[1].SetActive(false);
                        StartCoroutine(SummonWeapon());
                        weaponType = WeaponType.Medium;
                        break;
                    case WeaponState.Bow:
                        StartCoroutine(DisarmedWeapon());
                        weaponType = WeaponType.Light;
                        break;
                }
            }
        }
        if (!isDodge && !IsSkill)
        {
            Attack();
        }
        Variables.Instance.PlayerAnim.SetInteger(_weapon, weaponStateValue);
    }
    public bool IsCanAct(float useStamina)
    {
        if (playerNowStamina > useStamina)
            return true;
        return false;
    }
#region 조작
#region 이동
    public void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if (h != 0 && jumpCount == 0)
        {
            Variables.Instance.PlayerAnim.transform.localRotation = Quaternion.Euler(Vector3.up * Mathf.Sign(h) * 90f);
            Variables.Instance.PlayerAnim.SetFloat("Velocity Z", (playerRigid.velocity.x / playerStatus.MoveSpd) * Mathf.Sign(h));
        }  

        playerRigid.velocity = new Vector3(h * playerStatus.MoveSpd, playerRigid.velocity.y, playerRigid.velocity.z);

        Variables.Instance.PlayerAnim.SetBool(_moving, Mathf.Abs(h) > Mathf.Epsilon);
    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 3)
        {
            IsLand = false;
            if (jumpCount == 0)
            {
                jumpCount += 2;
            }
            else
                jumpCount++;

            Variables.Instance.PlayerAnim.SetInteger(_jumping, jumpCount);

            playerRigid.velocity = Vector3.zero;
            playerRigid.angularVelocity = Vector3.zero;
            playerRigid.AddForce(Vector3.up * playerStatus.JumpForce * 100f, ForceMode.Impulse);

            Variables.Instance.PlayerAnim.SetInteger(_triggerNum, (int)AnimState.Jump);
            Variables.Instance.PlayerAnim.SetTrigger(_trigger);
        }
        Variables.Instance.PlayerAnim.SetInteger(_jumping, jumpCount);
    }
    public void Dodge()
    {
        float h = Input.GetAxisRaw("Horizontal") * 0.5f;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isDodge = true;
            if (weaponRenderer[weaponStateValue].Length > 0)
                chararcterTrail.mat.SetColor("_GColor", weaponRenderer[weaponStateValue][0].material.color);
            else
                chararcterTrail.mat.SetColor("_GColor", whiteColor);
            chararcterTrail.OnTrail(0.3f);

            if(h!= 0)
            Variables.Instance.PlayerAnim.transform.localRotation = Quaternion.Euler(Vector3.up * Mathf.Sign(h) * 90f);

            Variables.Instance.PlayerAnim.gameObject.layer = LayerMask.NameToLayer("Dodge");

            Variables.Instance.PlayerAnim.SetInteger(_triggerNum, (int)AnimState.Dodge);
            Variables.Instance.PlayerAnim.SetTrigger(_trigger,()=>
            {
                Variables.Instance.PlayerAnim.gameObject.layer = LayerMask.NameToLayer("Player");

                Variables.Instance.PlayerAnim.SetInteger(_triggerNum, (int)AnimState.Idle,()=>
                {
                    isDodge = false;
                },0.4f
                );

            },0.4f
            );
        }
    }
#endregion

#region 공격
    public void Attack()
    {
        if (Input.GetKey(KeyCode.A) && IsLand)
        {
            if (IsCanAct((int)weaponType + 5) && !isAttack && !Variables.Instance.PlayerAnim.GetBool(_moving))
            {
                isAttack = true;
                playerNowStamina -= (int)weaponType + 5;

                Variables.Instance.PlayerAnim.SetInteger(_triggerNum, (int)AnimState.Attack);
                
                
                weapons[(int)weaponState].GetComponent<WeaponDefault>().Attack(attackMove, (returnValue) =>
                {
                    attackMove = weapons[(int)weaponState].GetComponent<WeaponDefault>().ReturnAttackMove(attackMove);
                    Variables.Instance.PlayerAnim.SetInteger(_action, attackMove);

                    Variables.Instance.PlayerAnim.SetTrigger(_trigger, () =>
                    {
                        isAttack = false;
                    }, (float)weaponType * 0.2f + 0.6f
                    );
                });

            }
            else if (IsCanAct((int)weaponType + 5) && !isAttack)
            {
                isAttack = true;
                playerNowStamina -= (int)weaponType + 5;

                Variables.Instance.PlayerAnim.SetInteger(_triggerNum, (int)AnimState.Attack);
                Variables.Instance.PlayerAnim.SetTrigger(_trigger, () =>
                {
                    isAttack = false;
                }, (float)weaponType * 0.3f + 0.6f
                    );

            }    
        }
    }

    public void Skill()
    {
        if (Input.GetKeyDown(KeyCode.R) && !Variables.Instance.PlayerAnim.GetBool(_moving) && jumpCount == 0)
        {
            IsSkill = true;
            Variables.Instance.PlayerAnim.SetInteger(_skill, 1);
            Variables.Instance.PlayerAnim.SetInteger(_triggerNum, (int)AnimState.Skill);

            Variables.Instance.PlayerAnim.SetTrigger(_trigger, ()=>
            {
                weapons[weaponStateValue].GetComponent<WeaponDefault>().SkillEffect();
                IsSkill = false;
            },1.5f);
        }
    }
    public void OnSkill()
    {
        weapons[weaponStateValue].GetComponent<WeaponDefault>().Skill();
    }
#endregion
#endregion

#region 시스템
    public IEnumerator RecoveryStamina()
    {
        while (true)
        {
            yield return null;
            if (!isAct && playerNowStamina < playerStatus.Stamina)
            {
                playerNowStamina += 0.5f;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
    public Vector3 ClampPos(Vector3 pos)
    {
        return new Vector3(
            pos.x
            , pos.y
            , Mathf.Clamp(pos.z, 0f, 0f)
            );

    }
    #region 무기 소환, 해제
    public IEnumerator SummonWeapon()
    {
        float timer = 1f;
        if (playerNowMp >= 20f)
        {
            timer = 1f;
            weaponStateValue = (int)weaponState;
            weapons[weaponStateValue + 1].SetActive(true);

            weaponState++;
            weaponStateValue++;

            attackMove = 0;

            Managers.Sound.Play("Player/SummonWeapon");
            if (jumpCount == 0)
            {
                Variables.Instance.PlayerAnim.SetInteger(_triggerNum, (int)AnimState.Idle);
                Variables.Instance.PlayerAnim.SetTrigger(_trigger);
            }

            while (timer >= -1)
            {
                yield return new WaitForSeconds(0.05f);
                timer -= 0.1f;
                weaponRenderer[weaponStateValue].ToList().ForEach(x => x.material.SetFloat(_alpha, timer));
                //weaponRenderer[weaponStateValue][0]?.material.SetFloat(_alpha, timer);
            }
        }
    }
    public IEnumerator DisarmedWeapon()
    {
        float timer = 0f;
        weaponStateValue = (int)weaponState;
        while (timer <= 1)
        {
            yield return new WaitForSeconds(0.05f);
            timer += 0.2f;
            weaponRenderer[weaponStateValue - 1].ToList().ForEach(x => x.material.SetFloat(_alpha, timer));
            //weaponRenderer[weaponStateValue - 1][0].material.SetFloat(_alpha, timer);
        }

        weaponState = WeaponState.None;
        weaponStateValue = (int)weaponState;

        yield return null;

        if (jumpCount == 0)
        {
            Variables.Instance.PlayerAnim.SetInteger(_triggerNum, (int)AnimState.Idle);
            Variables.Instance.PlayerAnim.SetTrigger(_trigger);
        }

        foreach (var weapon in weapons)
        weapon.SetActive(false);
    }
#endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsLand && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            IsLand = true;
            jumpCount = 0;
            if(Variables.Instance.PlayerAnim.GetInteger(_triggerNum) == (int)AnimState.Jump)
            Variables.Instance.PlayerAnim.SetTrigger(_trigger);
        }
    }
}
#endregion


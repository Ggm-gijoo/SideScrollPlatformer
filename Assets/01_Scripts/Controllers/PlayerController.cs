using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

enum AnimState
{
    Attack = 4,
    GetHit = 12,
    Jump = 18,
    Idle = 25,
    Knockback = 26,
    Knockdown = 27,
    Dodge = 28,
    Skill = 30,
}

enum WeaponState
{
    None = 0,
    Sword,
    Bow
}

enum WeaponType
{
    Light = 0,
    Medium,
    Heavy
}

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 SO")]
    [SerializeField] private PlayerStatus playerStatus;

    [Header("무기")]
    [SerializeField] GameObject[] weapons;

    [Header("공격 판정")]
    [SerializeField] BoxCollider[] attackCollider;

    [Header("무기 VFX")]
    [SerializeField] GameObject weaponVfxTrs;
    [SerializeField] GameObject[] weaponSkillVfx;

    Renderer[] weaponRenderer = new Renderer[100];
    
    private Animator playerAnim;
    private Rigidbody playerRigid;
    #region 스테이터스
    [Header("스테이터스")]
    public float playerNowHp;
    public float playerNowMp;
    public float playerNowStamina;

    public int jumpCount = 0;
    private int attackMove = 0;
    private int weaponStateValue = 0;
    public Vector3 trailRotationDist = new Vector3(0, 180, 0);

    private bool isAct = false;
    private bool isAttack = false;
    private bool isCanDash = false;
    private bool isDash = false;
    private bool isDodge = false;
    public bool isLand = false;
    private bool isSkill = false;

    private const string _alpha = "_Alpha";

    private readonly int _trigger = Animator.StringToHash("Trigger");
    private readonly int _triggerNum = Animator.StringToHash("TriggerNumber");
    private readonly int _moving = Animator.StringToHash("Moving");
    private readonly int _weapon = Animator.StringToHash("Weapon");
    private readonly int _action = Animator.StringToHash("Action");
    private readonly int _jumping = Animator.StringToHash("Jumping");
    private readonly int _skill = Animator.StringToHash("Skill");

    WeaponState weaponState = WeaponState.None;
    WeaponType weaponType = WeaponType.Light;
    #endregion

    #region 스타트 업데이트
    private void Start()
    {
        playerAnim = GetComponentInChildren<Animator>();
        playerRigid = GetComponent<Rigidbody>();

        int i = 0;
        foreach(var weapon in weapons)
        {
            weaponRenderer[i] = weapon.GetComponent<MeshRenderer>();
            weapon.SetActive(false);
            i++;
        }
        weaponVfxTrs.gameObject.SetActive(false);

        playerNowHp = playerStatus.Hp;
        playerNowMp = playerStatus.Mp;
        playerNowStamina = playerStatus.Stamina;

        StartCoroutine(RecoveryStamina());
    }

    private void Update()
    {
        if (!isAttack && !isDodge && !isSkill)
        {
            Move();
            Jump();
            Dodge();
            Skill();
            foreach (var attColl in attackCollider)
            {
                attColl.enabled = false;
            }
        }
        if (!playerAnim.GetBool(_moving))
        {
            Attack();
        }
   
        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch(weaponState)
            {
                case WeaponState.None:
                    StartCoroutine(SummonWeapon());
                    weaponType = WeaponType.Heavy;
                    break;
                case WeaponState.Sword:
                    weapons[0].SetActive(false);
                    StartCoroutine(SummonWeapon());
                    weaponType = WeaponType.Medium;
                    break;
                case WeaponState.Bow:
                    StartCoroutine(DisarmedWeapon());
                    weaponType = WeaponType.Light;
                    break;
            }
        }
        playerAnim.SetInteger(_weapon, weaponStateValue);
    }
    #endregion

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
        float h = Input.GetAxisRaw("Horizontal") * 0.5f;
        if (h != 0 && jumpCount == 0)
        {
            playerAnim.transform.localRotation = Quaternion.Euler(Vector3.up * Mathf.Sign(h) * 90f);
            playerAnim.SetFloat("Velocity Z", (playerRigid.velocity.x / playerStatus.MoveSpd) * Mathf.Sign(h));
        }
        else
        {
            playerAnim.SetFloat("Velocity Z", playerRigid.velocity.x / playerStatus.MoveSpd);
        }    
        isCanDash = Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(h) > Mathf.Epsilon && jumpCount == 0 && IsCanAct(1);
        isAct = isCanDash;

        if (isCanDash)
        {
            StartCoroutine(Dash());
            playerRigid.velocity = new Vector3(h * playerStatus.MoveSpd * 2, playerRigid.velocity.y, playerRigid.velocity.z);
        }
        else
        {
            isDash = false;
            playerRigid.velocity = new Vector3(h * playerStatus.MoveSpd, playerRigid.velocity.y, playerRigid.velocity.z);
        }

        playerAnim.SetBool(_moving, Mathf.Abs(h) > Mathf.Epsilon);
    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 3)
        {
            isLand = false;
            if (jumpCount == 0)
            {
                jumpCount += 2;
            }
            else
                jumpCount++;
            playerAnim.SetInteger(_jumping, jumpCount);
            playerRigid.AddForce(Vector3.up * playerStatus.JumpForce * 100f, ForceMode.Impulse);
            playerAnim.SetInteger(_triggerNum, (int)AnimState.Jump);
            playerAnim.SetTrigger(_trigger);
        }
        playerAnim.SetInteger(_jumping, jumpCount);
    }
    public void Dodge()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isDodge = true;
            playerAnim.SetInteger(_triggerNum, (int)AnimState.Dodge);
            playerAnim.SetTrigger(_trigger,()=>
            {
                isDodge = false;
            },1f
            );
        }
    }

    public IEnumerator Dash()
    {
        if (!isDash)
        {
            isDash = true;
            while (isCanDash)
            {
                yield return new WaitForSeconds(0.05f);
                playerNowStamina -= 1f;
            }
        }
    }
    #endregion

    #region 공격
    public void Attack()
    {
        if (Input.GetMouseButton(0) && isLand)
        {
            if (IsCanAct((int)weaponType + 5) && !isAttack)
            {
                isAttack = true;
                int attackAnimCount = 0;
                playerNowStamina -= (int)weaponType + 5;
                playerAnim.SetInteger(_triggerNum, (int)AnimState.Attack);
                switch (weaponState)
                {
                    case WeaponState.None:
                        attackAnimCount = 6;
                        Managers.Sound.Play($"Player/Sword_Swing_0{Random.Range(0, 2)}");
                        if ((attackMove + 1) % 2 == 0)
                        {
                            attackCollider[1].enabled = true;
                        }
                        else
                        {
                            attackCollider[0].enabled = true;
                        }
                        break;

                    case WeaponState.Sword:
                        attackAnimCount = 6;
                        StartCoroutine(WeaponVfxPlay());
                        attackCollider[2].enabled = true;
                        break;

                    case WeaponState.Bow:
                        attackAnimCount = 3;
                        break;
                }
                if (attackMove >= attackAnimCount)
                {
                    attackMove = 0;
                }
                attackMove++;

                playerAnim.SetInteger(_action, attackMove);

                playerAnim.SetTrigger(_trigger, () =>
                {
                    isAttack = false;
                    weaponVfxTrs.gameObject.SetActive(false);
                }, (float)weaponType * 0.2f + 0.6f
                );
            }
        }
    }

    public void Skill()
    {
        if (Input.GetKeyDown(KeyCode.R) && !playerAnim.GetBool(_moving) && jumpCount == 0)
        {
            isSkill = true;
            weaponSkillVfx?[weaponStateValue]?.SetActive(true);
            Time.timeScale = 0.7f;

            playerAnim.SetInteger(_skill, 1);
            playerAnim.SetInteger(_triggerNum, (int)AnimState.Skill);
            playerAnim.SetTrigger(_trigger, ()=>
            {
                Time.timeScale = 1f;
                weaponSkillVfx[weaponStateValue]?.SetActive(false);
                weaponSkillVfx[weaponStateValue].transform.localPosition = Vector3.zero;
                isSkill = false;
            },1.5f);
        }
    }
    #endregion
    #endregion

    #region 시스템
    public void WeaponSoundPlay()
    {
        Managers.Sound.Play($"Player/Sword{Random.Range(1, 7)}");
    }
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
    #region 무기 소환, 해제
    public IEnumerator SummonWeapon()
    {
        float timer = 1f;
        if (playerNowMp >= 20f)
        {
            timer = 1f;
            weaponStateValue = (int)weaponState;
            weapons[weaponStateValue].SetActive(true);

            weaponState++;
            weaponStateValue++;
            attackMove = 0;
            Managers.Sound.Play("Player/SummonWeapon");
            if (jumpCount == 0)
            {
                playerAnim.SetInteger(_triggerNum, (int)AnimState.Idle);
                playerAnim.SetTrigger(_trigger);
            }
            playerNowMp -= 20f;
            while (timer >= -1)
            {
                yield return new WaitForSeconds(0.05f);
                timer -= 0.1f;
                weaponRenderer[weaponStateValue - 1]?.material.SetFloat(_alpha, timer);
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
            weaponRenderer[weaponStateValue - 1].material.SetFloat(_alpha, timer);
        }

        weaponState = WeaponState.None;
        weaponStateValue = (int)weaponState;

        yield return null;

        if (jumpCount == 0)
        {
            playerAnim.SetInteger(_triggerNum, (int)AnimState.Idle);
            playerAnim.SetTrigger(_trigger);
        }

        foreach (var weapon in weapons)
        weapon.SetActive(false);
    }
    public IEnumerator WeaponVfxPlay()
    {
        weaponVfxTrs.transform.localRotation = Quaternion.Euler(0, 0, attackMove % 2 * 180f);
        yield return new WaitForSeconds(0.5f);
        weaponVfxTrs.gameObject.SetActive(true);
        WeaponSoundPlay();
        yield return new WaitForSeconds(0.4f);
        weaponVfxTrs.gameObject.SetActive(false);
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (!isLand && collision.collider.CompareTag("Ground"))
        {
            isLand = true;
            jumpCount = 0;
            playerAnim.SetTrigger(_trigger);
        }
    }
}
#endregion


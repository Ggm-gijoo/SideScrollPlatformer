using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AnimState
{
    Attack = 4,
    GetHit = 12,
    Dodge = 28,
    Jump = 18,
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

public class PlayerMove : MonoBehaviour
{
    [Header("플레이어 SO")]

    [SerializeField] private PlayerStatus playerStatus;

    [Space(3f)]
    [Header("무기")]

    [SerializeField] GameObject[] weapons;

    [Space(3f)]
    [Header("공격 판정")]

    [SerializeField] BoxCollider[] attackCollider;


    [SerializeField] ParticleSystem trail;

    Renderer[] weaponRenderer = new Renderer[100];
    
    private Animator playerAnim;
    private Rigidbody playerRigid;
    #region 스테이터스
    public float playerNowHp;
    public float playerNowMp;
    public float playerNowStamina;

    public int jumpCount = 0;
    private int attackMove = 0;
    private int weaponStateValue = 0;

    private bool isAct = false;
    private bool isAttack = false;
    private bool isCanDash = false;
    private bool isDash = false;
    public bool isLand = false;

    private const string _alpha = "_Alpha";

    private readonly int _trigger = Animator.StringToHash("Trigger");
    private readonly int _triggerNum = Animator.StringToHash("TriggerNumber");
    private readonly int _moving = Animator.StringToHash("Moving");
    private readonly int _weapon = Animator.StringToHash("Weapon");
    private readonly int _action = Animator.StringToHash("Action");
    private readonly int _jumping = Animator.StringToHash("Jumping");

    WeaponState weaponState = WeaponState.None;
    WeaponType weaponType = WeaponType.Light;
    #endregion


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

        //trail = weapons[0].GetComponentInChildren<TrailRenderer>();
        trail.gameObject.SetActive(false);

        playerNowHp = playerStatus.Hp;
        playerNowMp = playerStatus.Mp;
        playerNowStamina = playerStatus.Stamina;

        StartCoroutine(RecoveryStamina());
    }

    private void Update()
    {
        if (!isAttack)
        {
            Move();
            Jump();
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

    public bool IsCanAct(float useStamina)
    {
        if (playerNowStamina > useStamina)
            return true;
        return false;
    }
#region 이동
    public void Move()
    {
        float h = Input.GetAxisRaw("Horizontal") * 0.5f;
        playerAnim.SetFloat("Velocity Z", playerRigid.velocity.x / playerStatus.MoveSpd);
        isCanDash = Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(h) > Mathf.Epsilon && IsCanAct(1);
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
    public void Attack()
    {
        if (Input.GetMouseButton(0) && isLand)
        {
            if (IsCanAct((int)weaponType + 5) && !isAttack)
            {
                isAttack = true;
                playerNowStamina -= (int)weaponType + 5;
                playerAnim.SetInteger(_triggerNum, (int)AnimState.Attack);
                if (attackMove >= 6)
                {
                    attackMove = 0;
                }
                attackMove++;

                if (weaponState == WeaponState.Sword)
                {
                    StartCoroutine(TrailWeapon());
                    attackCollider[2].enabled = true;
                }
                else if (attackMove % 2 == 0)
                {
                    attackCollider[1].enabled = true;
                }
                else
                {
                    attackCollider[0].enabled = true;
                }

                playerAnim.SetInteger(_action, attackMove);
                playerAnim.SetTrigger(_trigger, () =>
                {
                    isAttack = false;
                    trail.gameObject.SetActive(false);
                }, (float)weaponType * 0.25f + 0.5f
                );
            }
        }
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
            playerNowMp -= 20f;
            while (timer >= -1)
            {
                yield return new WaitForSeconds(0.05f);
                timer -= 0.1f;
                weaponRenderer[weaponStateValue - 1].material.SetFloat(_alpha, timer);
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
            timer += 0.1f;
            weaponRenderer[weaponStateValue - 1].material.SetFloat(_alpha, timer);
            if (weaponRenderer[weaponStateValue - 1].material.GetFloat(_alpha) >= 0.7f)
            {
                weaponState = WeaponState.None;
            }
        }
        foreach(var weapon in weapons)
        weapon.SetActive(false);
    }
    public IEnumerator TrailWeapon()
    {
        float timer = -1f;
        yield return new WaitForSeconds(0.4f);
        trail.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        while (timer <= -0.3f)
        {
            yield return new WaitForSeconds(0.025f);
            timer += 0.1f;
            trail.material.SetFloat(_alpha, timer);
        }
        trail.gameObject.SetActive(false);
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


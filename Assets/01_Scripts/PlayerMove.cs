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
    Sword = 1,
    Bow = 2
}

enum WeaponType
{
    Light = 0,
    Medium,
    Heavy
}

public class PlayerMove : MonoBehaviour
{
    [Space]
    [SerializeField]
    private PlayerStatus playerStatus;
    [SerializeField]
    GameObject[] weapons;
    [SerializeField]
    BoxCollider[] attackCollider;
    TrailRenderer trail;
    Renderer[] weaponRenderer = new Renderer[100];
    public Animator playerAnim;
    private Rigidbody playerRigid;

    public float playerNowHp;
    public float playerNowMp;
    public float playerNowStamina;

    private float trailTimer = -1f;
    public int jumpCount = 0;
    private int attackMove = 0;

    private bool isAct = false;
    private bool isAttack = false;
    private bool isCanDash = false;
    private bool isDash = false;
    public bool isLand = false;

    
    WeaponState weaponState = WeaponState.None;
    WeaponType weaponType = WeaponType.Light;

    private void Start()
    {
        playerAnim = GetComponentInChildren<Animator>();
        playerRigid = GetComponent<Rigidbody>();
        int i = 0;
        foreach(var weapon in weapons)
        {
            print(weapon.GetComponent<MeshRenderer>());
            weaponRenderer[i] = weapon.GetComponent<MeshRenderer>();
            weapon.SetActive(false);
            i++;
            Debug.Log(i);
        }
        i = 0;

        trail = weapons[0].GetComponentInChildren<TrailRenderer>();
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
        if (!playerAnim.GetBool("Moving"))
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
        playerAnim.SetInteger("Weapon", (int)weaponState);
    }

    public bool IsCanAct(float useStamina)
    {
        if (playerNowStamina > useStamina)
            return true;
        return false;
    }
    //스태미나가 계속 회복되는 조건
    //현재 스태미나를 소모하는 행동중이 아닐 때

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

        if (Mathf.Abs(h) > Mathf.Epsilon)
        {
            playerAnim.SetBool("Moving", true);
        }
        else
            playerAnim.SetBool("Moving", false);
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
            playerAnim.SetInteger("Jumping", jumpCount);
            playerRigid.AddForce(Vector3.up * playerStatus.JumpForce * 100f,ForceMode.Impulse);
            playerAnim.SetInteger("TriggerNumber", (int)AnimState.Jump);
            playerAnim.SetTrigger("Trigger");
        }
        playerAnim.SetInteger("Jumping", jumpCount);
    }

    public void Attack()
    {
        if (Input.GetMouseButtonDown(0) && isLand)
        {
            if (IsCanAct((int)weaponState + 5) && !isAttack)
            {
                isAttack = true;
                playerNowStamina -= (int)weaponType + 5;
                playerAnim.SetInteger("TriggerNumber", (int)AnimState.Attack);
                if (attackMove >= 6)
                {
                    attackMove = 0;
                }
                attackMove++;

                if(weaponState == WeaponState.Sword)
                {
                    StopCoroutine(TrailWeapon());
                    trailTimer = -1f;
                    StartCoroutine(TrailWeapon());
                    attackCollider[2].enabled = true;
                }
                else if(attackMove % 2 == 0)
                {
                    attackCollider[1].enabled = true;
                }
                else
                {
                    attackCollider[0].enabled = true;
                }

                playerAnim.SetInteger("Action", attackMove);
                playerAnim.SetTrigger("Trigger", () =>
                {
                    isAttack = false;
                    trail.gameObject.SetActive(false);
                }, (float)weaponType * 0.25f + 0.5f
                );
            }
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

    public IEnumerator SummonWeapon()
    {
        float timer = 1f;
        if (playerNowMp >= 20f)
        {
            weapons[(int)weaponState].SetActive(true);
            //Debug.Log(weaponState);
            Debug.Log(weaponRenderer[(int)weaponState].material.GetFloat("_Float"));
            weaponState++;
            playerNowMp -= 20f;
            while (weaponRenderer[(int)weaponState - 1].material.GetFloat("_Float") >= -1)
            {
                yield return new WaitForSeconds(0.05f);
                timer -= 0.1f;
                weaponRenderer[(int)weaponState - 1].material.SetFloat("_Float", timer);
            }
        }
    }
    public IEnumerator DisarmedWeapon()
    {
        float timer = 0f;
        while (weaponRenderer[(int)weaponState].material.GetFloat("_Float") <= 1)
        {
            yield return new WaitForSeconds(0.05f);
            timer += 0.1f;
            print(weaponRenderer[(int)weaponState].material);
            weaponRenderer[(int)weaponState].material.SetFloat("_Float", timer);
            if (weaponRenderer[(int)weaponState].material.GetFloat("_Float") >= 0.7f)
            {
                weaponState = WeaponState.None;
            }
        }
        foreach(var weapon in weapons)
        weapon.SetActive(false);
    }
    public IEnumerator TrailWeapon()
    {
        trailTimer = -1f;
        yield return new WaitForSeconds(0.25f);
        trail.gameObject.SetActive(true);
        while (trailTimer <= -0.4f)
        {
            yield return new WaitForSeconds(0.05f);
            trailTimer += 0.05f;
            trail.material.SetFloat("_Float", trailTimer);
        }
        trail.gameObject.SetActive(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isLand && collision.collider.CompareTag("Ground"))
        {
            isLand = true;
            jumpCount = 0;
            playerAnim.SetTrigger("Trigger");
        }
    }
}


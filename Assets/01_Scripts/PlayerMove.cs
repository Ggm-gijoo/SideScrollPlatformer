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
    Sword = 1
}

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    private PlayerStatus playerStatus;
    [SerializeField]
    GameObject weapon;
    Renderer weaponRenderer;
    private Animator playerAnim;
    private Rigidbody playerRigid;

    public float playerNowHp;
    public float playerNowMp;
    public float playerNowStamina;

    private int jumpCount = 0;
    private int attackMove = 0;

    private bool isAct = false;
    private bool isCanDash = false;
    private bool isDash = false;
    private bool isLand = false;

    WeaponState weaponState = WeaponState.None;

    private void Start()
    {
        playerAnim = GetComponentInChildren<Animator>();
        playerRigid = GetComponent<Rigidbody>();
        weaponRenderer = weapon.GetComponent<Renderer>();

        playerNowHp = playerStatus.Hp;
        playerNowMp = playerStatus.Mp;
        playerNowStamina = playerStatus.Stamina;

        StartCoroutine(RecoveryStamina());
    }
    private void Update()
    {
        Move();
        Jump();
        Attack();

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(weaponState == WeaponState.None)
            {
                StartCoroutine(SummonWeapon());
            }
            else
            {
                StartCoroutine(DisarmedWeapon());
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
        int playerDir = (int)Mathf.Sign(h);

        isCanDash = Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(h) > Mathf.Epsilon && IsCanAct(1);
        isAct = isCanDash;

        if (isCanDash)
        {
            StartCoroutine(Dash());
            playerRigid.velocity = new Vector3(h * playerStatus.MoveSpd * 2f, playerRigid.velocity.y, playerRigid.velocity.z);
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
            if(jumpCount == 0)
            {
                jumpCount += 2;
            }
            else
            jumpCount++;
            playerAnim.SetInteger("Jumping", jumpCount);

            playerAnim.SetInteger("TriggerNumber", (int)AnimState.Jump);
            playerRigid.AddForce(Vector3.up * playerStatus.JumpForce * 100, ForceMode.Impulse);
            playerAnim.SetTrigger("Trigger");
        }
        playerAnim.SetInteger("Jumping", jumpCount);
    }

    public void Attack()
    {
        if (Input.GetMouseButtonDown(0) && playerAnim.GetInteger("Jump") == 0)
        {
            playerAnim.SetInteger("TriggerNumber", (int)AnimState.Attack);
            if (attackMove >= 6)
            {
                attackMove = 0;
            }
            attackMove++;
            playerAnim.SetInteger("Action", attackMove);
            playerAnim.SetTrigger("Trigger");
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
                Debug.Log($"현재 스태미나 : {playerNowStamina}");
                playerNowStamina += 0.5f;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    public IEnumerator SummonWeapon()
    {
        float timer = 1f;
        while (weaponRenderer.material.GetFloat("_Float") >= -1)
        {
            yield return new WaitForSeconds(0.05f);
            timer -= 0.05f;
            weaponRenderer.material.SetFloat("_Float", timer);
            if(weaponRenderer.material.GetFloat("_Float") <= -0.3f)
            {
                weaponState = WeaponState.Sword;
            }
        }
    }
    public IEnumerator DisarmedWeapon()
    {
        float timer = 0f;
        while (weaponRenderer.material.GetFloat("_Float") <= 1)
        {
            yield return new WaitForSeconds(0.05f);
            timer += 0.1f;
            weaponRenderer.material.SetFloat("_Float", timer);
            if (weaponRenderer.material.GetFloat("_Float") >= 0.7f)
            {
                weaponState = WeaponState.None;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Ground") && !isLand)
        {
            isLand = true;
            playerAnim.SetTrigger("Trigger");
            jumpCount = 0;
        }
    }
}


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
    [Space]
    [SerializeField]
    private PlayerStatus playerStatus;
    [SerializeField]
    GameObject weapon;
    [SerializeField]
    BoxCollider[] attackCollider;
    Renderer weaponRenderer;
    public Animator playerAnim;
    private Rigidbody playerRigid;
    //private CharacterController playerController;

    public float playerNowHp;
    public float playerNowMp;
    public float playerNowStamina;

    public int jumpCount = 0;
    private int attackMove = 0;
    //private float yDir = 0;

    private bool isAct = false;
    private bool isAttack = false;
    private bool isCanDash = false;
    private bool isDash = false;
    public bool isLand = false;

    //Vector3 moveDir = Vector3.zero;

    WeaponState weaponState = WeaponState.None;

    private void Start()
    {
        playerAnim = GetComponentInChildren<Animator>();
        //playerController = GetComponent<CharacterController>();
        playerRigid = GetComponent<Rigidbody>();
        weaponRenderer = weapon.GetComponent<Renderer>();

        playerNowHp = playerStatus.Hp;
        playerNowMp = playerStatus.Mp;
        playerNowStamina = playerStatus.Stamina;

        weapon.SetActive(false);
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
        Attack();
        //if(!playerController.isGrounded)
        //yDir += Physics.gravity.y * Time.deltaTime;
        //playerController.Move(moveDir * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Q))
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
        playerAnim.SetFloat("Velocity Z", playerRigid.velocity.x/*playerController.velocity.x*/ / playerStatus.MoveSpd);

        isCanDash = Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(h) > Mathf.Epsilon && IsCanAct(1);
        isAct = isCanDash;

        if (isCanDash)
        {
            StartCoroutine(Dash());
            //moveDir = new Vector3(h * playerStatus.MoveSpd * 2f, yDir, 0);
            playerRigid.velocity = new Vector3(h * playerStatus.MoveSpd * 2, playerRigid.velocity.y, playerRigid.velocity.z);
        }
        else
        {
            isDash = false;
            playerRigid.velocity = new Vector3(h * playerStatus.MoveSpd, playerRigid.velocity.y, playerRigid.velocity.z);
            //moveDir = new Vector3(h * playerStatus.MoveSpd, yDir, 0);
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
            //yDir = playerStatus.JumpForce;
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
                playerNowStamina -= (int)weaponState + 5;
                playerAnim.SetInteger("TriggerNumber", (int)AnimState.Attack);
                if (attackMove >= 6)
                {
                    attackMove = 0;
                }
                attackMove++;
                if(weaponState == WeaponState.Sword)
                {
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
                }, (float)weaponState * 0.5f + 0.5f
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
                Debug.Log($"현재 스태미나 : {playerNowStamina}");
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
            weapon.SetActive(true);
            playerNowMp -= 20f;
            while (weaponRenderer.material.GetFloat("_Float") >= -1)
            {
                yield return new WaitForSeconds(0.05f);
                timer -= 0.05f;
                weaponRenderer.material.SetFloat("_Float", timer);
                if (weaponRenderer.material.GetFloat("_Float") <= -0.3f)
                {
                    weaponState = WeaponState.Sword;
                }
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
        weapon.SetActive(false);
    }
    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    if(!isLand && playerController.isGrounded)
    //    {
    //        isLand = true;
    //        jumpCount = 0;
    //        playerAnim.SetTrigger("Trigger");
    //    }
    //}
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


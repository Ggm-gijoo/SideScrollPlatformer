using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AnimState
{
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
    private Animator playerAnim;
    private Rigidbody playerRigid;

    public float playerNowHp;
    public float playerNowMp;
    public float playerNowStamina;

    private bool isAct = false;
    private bool isCanDash = false;
    private bool isDash = false;

    private void Start()
    {
        playerAnim = GetComponentInChildren<Animator>();
        playerRigid = GetComponent<Rigidbody>();

        playerNowHp = playerStatus.Hp;
        playerNowMp = playerStatus.Mp;
        playerNowStamina = playerStatus.Stamina;

        StartCoroutine(RecoveryStamina());
    }
    private void Update()
    {
        Move();
        Jump();
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
        if (Input.GetKeyDown(KeyCode.Space) /*&& playerRigid.velocity.y < 1*/)
        {
            Debug.Log("점프시도");
            //playerRigid.AddForce(Vector3.up * playerStatus.JumpForce * 100, ForceMode.Impulse);
            playerAnim.SetTrigger("Trigger");
            playerAnim.SetFloat("TriggerNumber", 18);
            playerAnim.SetFloat("Jumping", 1);
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
}


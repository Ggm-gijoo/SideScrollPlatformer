using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarManager : MonoBehaviour
{
    [Space]
    [SerializeField]
    private Transform player;
    [SerializeField]
    private PlayerStatus playerStatus;

    private PlayerMove playerMove;

    [SerializeField]
    private Image hpBarImage;
    [SerializeField]
    private Image mpBarImage;
    [SerializeField]
    private Image staminaBarImage;

    private void Start()
    {
        playerMove = player.GetComponent<PlayerMove>();
    }

    private void Update()
    {
        staminaBarImage.fillAmount = playerMove.playerNowStamina / playerStatus.Stamina;
        hpBarImage.fillAmount = playerMove.playerNowHp / playerStatus.Hp;
        mpBarImage.fillAmount = playerMove.playerNowMp / playerStatus.Mp;
    }
}

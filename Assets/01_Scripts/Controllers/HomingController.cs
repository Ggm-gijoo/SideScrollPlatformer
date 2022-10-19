using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using Cinemachine;
using UnityEngine.Rendering;
using DG.Tweening;

public class HomingController : MonoBehaviour
{
    [SerializeField] private float homingRadius = 3f;

    private List<Transform> targetList = new List<Transform>();

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
            OnHoming();
    }
    public void OnHoming()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, homingRadius);

        foreach(var col in cols)
        {
            if (col.CompareTag("Enemy"))
            {
                transform.DOMove(col.transform.forward, 0.01f);
                targetList.Add(col.transform);
            }
        }

        if (targetList.Count > 0)
            Time.timeScale = 0.05f;
    }
}

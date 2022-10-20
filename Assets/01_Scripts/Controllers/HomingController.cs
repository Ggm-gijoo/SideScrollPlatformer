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
    [SerializeField] private CanvasGroup aimCanvas;

    private bool flag = false;
    private bool isHoming = false;

    private List<Transform> targetList = new List<Transform>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isHoming)
                OnHoming();
            else
                flag = false;
        }
    }
    public void SetTarget(Transform target ,bool on = true)
    {
        aimCanvas.transform.position = Camera.main.WorldToScreenPoint(target.position + Vector3.up);
        aimCanvas.alpha = on? 1 : 0;
    }
    public void OnHoming()
    {
        isHoming = true;
        Collider[] cols = Physics.OverlapSphere(transform.position, homingRadius);

        foreach(var col in cols)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                targetList.Add(col.transform);
            }
        }

        if (targetList.Count > 0)
        {
            Time.timeScale = 0.01f;
            flag = true;
            StartCoroutine(SelectTarget());
            SetTarget(targetList[0].transform);
        }
        else
            isHoming = false;
    }
    public IEnumerator SelectTarget()
    {
        int idx = 0;
        float timer = 0;
        float h = Input.GetAxisRaw("Horizontal");
        while (flag)
        {
            timer += Time.unscaledDeltaTime;
            h = Input.GetAxisRaw("Horizontal");
            if (h != 0 && timer >= 0.2f)
            {
                timer = 0f;
                if ((int)h + idx < 0)
                    idx = targetList.Count - 1;
                else
                    idx = ((int)h + idx) % targetList.Count;
                SetTarget(targetList[idx].transform);
            }
            yield return null;
        }
        Time.timeScale = 1f;
        transform.DOMove(targetList[idx].position - targetList[idx].right * 0.7f, 0.01f);
        SetTarget(targetList[idx].transform, false);
        isHoming = false;
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob_00_Slime : MonsterDefault
{
    private Animator slimeAnim;
    private void Awake()
    {
        slimeAnim = GetComponent<Animator>();
    }
    private void Update()
    {
        InvokeRepeating("Attack", 2f, 2f);
    }
    public override void Attack()
    {
        Debug.Log("Attack!");
        Collider[] cols = Physics.OverlapSphere(transform.position, 5f);

        foreach(var col in cols)
        {
            if(col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                StartCoroutine(Damage());
            }
            else if(col.gameObject.layer == LayerMask.NameToLayer("Dodge"))
            {
                col.GetComponent<HomingController>().OnHoming();
            }
        }
    }

    private IEnumerator Damage()
    {
        //Time.timeScale = 0.001f;
        yield return new WaitForSecondsRealtime(3f);
        //Debug.Log("AA");
        //Time.timeScale = 1f;
    }

}

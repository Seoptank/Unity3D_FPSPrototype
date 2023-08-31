using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePursuit : ZombieState
{

    [SerializeField]
    private float        recognitionRange = 8.0f;     // Player인식 범위
    [SerializeField]
    private float        attRange = 1.0f;
    [SerializeField]
    private float        runSpeed = 6.0f;
    [SerializeField]
    private ZombieIdle   idle;
    [SerializeField]
    private ZombieAttack attack;
    [SerializeField]
    private Transform    target;

    public override ZombieState RunCurState()
    {
        if (ChangeStateToAttack() == true)
        {
            return attack;
        }
        else if (ChangeStateToIdle() == true)
        {
            return idle;
        }
        else
        {
            return this;
        }
    }
    public bool ChangeStateToIdle()
    {
        float dis = Vector3.Distance(target.position, transform.position);

        if (dis > recognitionRange) return true;
    
        return false;
    }
    public bool ChangeStateToAttack()
    {
        float dis = Vector3.Distance(target.position, transform.position);

        if (dis <= attRange) return true;
        return false;
    }
    
    private void OnDrawGizmos()
    {
        // 공격 범위
        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attRange);
    }
}

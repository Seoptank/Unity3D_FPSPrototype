using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdle : ZombieState
{
    [SerializeField]
    private ZombiePursuit       pursuit;                    // "Pursuit"상태
    [SerializeField]
    private float               RecognitionRange = 8.0f;    // Player인식 범위
    [SerializeField]
    private Transform           target;                     // target = Player

    public override ZombieState RunCurState()
    {
        if (ChangeStateToWander() == true)
        {
            return pursuit;
        }
        else
        {
            return this;
        }
    }
    private bool ChangeStateToWander()
    {
        float dis = Vector3.Distance(target.position, transform.position);

        if(dis <= RecognitionRange) return true;

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RecognitionRange);
    }

}

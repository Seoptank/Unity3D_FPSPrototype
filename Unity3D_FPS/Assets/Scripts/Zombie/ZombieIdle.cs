using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdle : ZombieState
{
    private bool isIdle = false;

    public override ZombieState RunCurState()
    {
        if (OnPursuit() == true)
        {
            return Pursuit;
        }
        else if(OnWander() == true)
        {
            return Wander;
        }
        else
        {
            return this;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, recognitionRange);
    }

}

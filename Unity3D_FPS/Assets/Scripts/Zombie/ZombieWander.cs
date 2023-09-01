using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieWander : ZombieState
{
    [Header("State")]
    [SerializeField]
    private ZombieIdle      idle;
    [SerializeField]
    private ZombiePursuit   pursuit;

    private float           dis;

    public override ZombieState RunCurState()
    {
        if (OnIdle() == true)
        {
            return idle;
        }
        else if(OnPursuit() == true)
        {
            return pursuit;
        }
        else
        {
            return this;
        }

    }

    public bool OnIdle()
    {
        dis = Vector3.Distance(target.position, transform.position);

        if(dis >= pursuitLimitRange)
        {
            return true;
        }
        return false;
    }

    public bool OnPursuit()
    {
        dis = Vector3.Distance(target.position, transform.position);

        if(dis <= recognitionRange)
        {
            return true;
        }
        return false;
    }


}

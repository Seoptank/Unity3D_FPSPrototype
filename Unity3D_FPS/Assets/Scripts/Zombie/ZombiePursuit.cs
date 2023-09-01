using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePursuit : ZombieState
{
    [Header("State")]


    public override ZombieState RunCurState()
    {
        if (OnAttack() == true)
        {
            return Attack;
        }
        else if (OnIdle() == true)
        {
            return Idle;
        }
        else
        {
            return this;
        }
    }
}

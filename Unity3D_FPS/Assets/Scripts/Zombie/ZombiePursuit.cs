using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePursuit : ZombieState
{
    public ZombieAttack attack;
    public bool isAttRange;
    public override ZombieState RunCurState()
    { 
        if(isAttRange)
        {
            return attack;
        }
        else
        {
            return this;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack : ZombieState
{
    public override ZombieState RunCurState()
    {
        Debug.Log("АјАн!");
        return this;
    }
}

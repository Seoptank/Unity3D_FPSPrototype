using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieWander : ZombieState
{
    public override ZombieState RunCurState()
    {
        return this;
    }
}

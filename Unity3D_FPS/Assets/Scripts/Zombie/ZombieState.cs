using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ZombieState : MonoBehaviour
{
    public abstract ZombieState RunCurState();
}

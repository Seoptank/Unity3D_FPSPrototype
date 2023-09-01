using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ZombieState : MonoBehaviour
{
    [SerializeField]
    protected float     recognitionRange = 8.0f;    // Player인식 범위
    [SerializeField]
    protected float     pursuitLimitRange = 9.0f;
    [SerializeField]
    protected float     attRange = 1.0f;
    [SerializeField]
    protected Transform target;

    public abstract ZombieState RunCurState();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdle : ZombieState
{
    [SerializeField]
    private ZombiePursuit       pursuit;                    // "Pursuit"상태
    [SerializeField]
    private float               RecognitionRange = 8.0f;     // Player인식 범위
    [SerializeField]
    private Transform           target;                     // target = Player

    private bool                isRecognize = false;
    public override ZombieState RunCurState()
    {
        if (isRecognize == true)
        {
            return pursuit;
        }
        else
        {
            return this;
        }
    }

    public 
}

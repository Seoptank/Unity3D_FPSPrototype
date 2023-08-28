using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdle : ZombieState
{
    [SerializeField]
    private ZombiePursuit       pursuit;                    // "Pursuit"����
    [SerializeField]
    private float               RecognitionRange = 8.0f;     // Player�ν� ����
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

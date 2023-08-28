using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieStateManager : MonoBehaviour
{
    [SerializeField]
    private ZombieState curState;

    private void Update()
    {
        RunStateMachine();
    }

    private void RunStateMachine()
    {
        ZombieState nextState = curState.RunCurState();

        if(nextState != null)
        {
            // 다음 스테이트로 변경하는 코드
            SwitchNextState(nextState);
        }

    }

    private void SwitchNextState(ZombieState nextState)
    {
        curState = nextState;
    }

}

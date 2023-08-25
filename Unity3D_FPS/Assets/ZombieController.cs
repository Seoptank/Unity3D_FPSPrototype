using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ZombieState
{
    None = -1,
    Idle = 0,
    Wander,
    Pursuit,
}
public class ZombieController : MonoBehaviour
{
    [Header("Pursuit")]
    [SerializeField]
    private float           targetRecognitionRange = 5;
    [SerializeField]
    private float           pursuitLimitRange = 8;
    [SerializeField]
    private Transform       target;

    [Header("Attack")]
    [SerializeField]
    private float           attRange = 5;
    [SerializeField]
    private float           attRate = 1;

    private ZombieState     zombieState = ZombieState.None;
    private float           lastAttTime = 0;

    private Status          status;
    private NavMeshAgent    nav;

    private void Awake()
    {
        status = GetComponent<Status>();
        nav    = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        nav.updateRotation = false;
        ChangeState(ZombieState.Idle);
    }


    //private void OnDisable()
    //{
    //    // 적이 비활성화 될때 현재 상태 종료하고,상태 None으로 설정
    //    StopCoroutine(zombieState.ToString());
    //
    //    zombieState = ZombieState.None;
    //}

    //==================================================================
    // 상태 변경 함수
    //==================================================================
    public void ChangeState(ZombieState newState)
    {
        if (zombieState == newState) return;

        StopCoroutine(zombieState.ToString());
        zombieState = newState;
        StartCoroutine(zombieState.ToString());
    }
    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1~4초 시간 대기
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        // 상태를 "배회"로 변경
        ChangeState(ZombieState.Wander);
    }
    private void CalculateDisToTargetAndSelectState()
    {
        if (target == null) return;

        float dis = Vector3.Distance(target.position, transform.position);

        if(dis <= targetRecognitionRange)
        {
            ChangeState(ZombieState.Pursuit);
        }
        if(dis <= pursuitLimitRange)
        {
            ChangeState(ZombieState.Wander);
        }
    }

    //==================================================================
    // 상태 함수
    //==================================================================
    private IEnumerator Idle()
    {
        // 자동으로 "Wander"로 변경하는 함수 실행
        StartCoroutine("AutoChangeFromIdleToWander");

        while(true)
        {
            // 타겟과의 거리에 따라 행동 선택하는 함수 실행
            CalculateDisToTargetAndSelectState();

            yield return null;
        }
    }
    private IEnumerator Wander()
    {
        float curTime = 0;
        float maxTime = 10;

        // 이동 속도 설정
        nav.speed = status.walkSpeed;

        // 목표 위치 설정
        nav.SetDestination(CalculateWanderPos());

        // 목표 위치로 회전
        // 방향값 = 이동할 목표 지점 - 이동 시작 지점
        Vector3 to = new Vector3(nav.destination.x, 0, nav.destination.z);
        Vector3 from = new Vector3(nav.transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        while(true)
        {
            curTime += Time.deltaTime;

            to = new Vector3(nav.destination.x, 0, nav.destination.z);
            from = new Vector3(nav.transform.position.x, 0, transform.position.z);

            if((to - from).sqrMagnitude<0.01f || curTime >= maxTime)
            {
                ChangeState(ZombieState.Idle);
            }
            CalculateDisToTargetAndSelectState();
            yield return null;
        }    
    }

    private IEnumerator Pursuit()
    {
        while(true)
        {
            nav.speed = status.RunSpeed;

            nav.SetDestination(target.position);

            LookRotationToTarget();

            CalculateDisToTargetAndSelectState();

            yield return null;
        }
    }
    private IEnumerator Attack()
    {
        nav.ResetPath();

        while(true)
        {
            LookRotationToTarget();

            CalculateDisToTargetAndSelectState();

            if(Time.time - lastAttTime > attRate)
            {

            }
        }
    }

    //==================================================================
    // 계산 함수
    //==================================================================
    private Vector3 CalculateWanderPos()
    {
        float wanderR    = 10;
        int wanderJit    = 0;
        int wanderJitMin = 0;
        int wanderJitMax = 360;

        Vector3 rangePos    = Vector3.zero;
        Vector3 rangeScale  = Vector3.one * 100.0f;

        // 선택된 각도(wnaderJit)에 위치한 좌표를 목표지점으로 설정
        wanderJit = Random.Range(wanderJitMin, wanderJitMax);
        Vector3 targetPos = transform.position + SetAngle(wanderR, wanderJit);

        // 구역을 벗어나지 않도록
        targetPos.x = Mathf.Clamp(targetPos.x,
                                  rangePos.x - rangeScale.x * 0.5f,
                                  rangePos.x + rangeScale.x * 0.5f);
        targetPos.y = 0.0f;
        targetPos.z = Mathf.Clamp(targetPos.z,
                                  rangePos.z - rangeScale.z * 0.5f,
                                  rangePos.z + rangeScale.z * 0.5f);
        return targetPos;
    }

    private Vector3 SetAngle(float r,int angle)
    {
        Vector3 position = Vector3.zero;
        position.x = Mathf.Cos(angle) * r;
        position.z = Mathf.Sin(angle) * r;

        return position;
    }
    private void LookRotationToTarget()
    {
        // 목표 위치
        Vector3 to = new Vector3(target.position.x, 0.0f, target.position.z);
        Vector3 from = new Vector3(transform.position.x,0.0f,transform.position.z);

        // 돌기
        transform.rotation = Quaternion.LookRotation(to - from);
    }

    //==================================================================
    // 기즈모 함수
    //==================================================================
    private void OnDrawGizmos()
    {
        // "배회"상태일 때 이동경로 표시 
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, nav.destination - transform.position);

        // 인식 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // 추적 범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);
    }
}

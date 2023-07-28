using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public  enum EnemyState
{
    NONE = -1,
    IDLE = 0,
    WANDER,
}

public class EnemyFMS : MonoBehaviour
{
    private EnemyState      enemyState = EnemyState.NONE;

    private Status          status;
    private NavMeshAgent    navMeshAgent;

    private void Awake()
    {
        status       = GetComponent<Status>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        // NavMeshAgent 컴포넌트에서 회전을 업데이트하지 않도록 설정
        navMeshAgent.updateRotation = false;
    }

    private void OnEnable()
    {
        // 적이 활성화 됭 때 적의 상태를 "대기로 설정"
        ChangeState(EnemyState.IDLE);
    }

    private void OnDisable()
    {
        //적이 비활성화될 때 현재 재생중인 상태를 종료하고, 상태를 "NONE"으로 설정
        StopCoroutine(enemyState.ToString());

        enemyState = EnemyState.NONE;
    }

    public void ChangeState(EnemyState newState)
    {
        // 현재 재생중인 상태와 바꾸려고 하는 상태가 같으면 바꿀 필요가 없기 때문에 return
        if (enemyState == newState) return;

        // 이전에 재생중이던 상태 종료
        StopCoroutine(enemyState.ToString());
        // 현재 적의 상태를 newState로 설정
        enemyState = newState;
        // 새로운 상태 설정
        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        // n초 후에 "배회" 상태로 변경하는 코드 실행
        StartCoroutine("AutoChangeFromIdleToWander");

        while(true)
        {
            // "대기 상태일 때 하는 행동"

            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1~4초 시간 대기
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        // 상태를 "배회"로 변경
        ChangeState(EnemyState.WANDER);
    }

    private IEnumerator Wander()
    {
        float curTime = 0;
        float maxTime = 10;

        // 이동 속도 설정
        navMeshAgent.speed = status.WalkSpeed;

        //목표 위치 설정
        navMeshAgent.SetDestination(CalculateWanderPosition());

        // 목표 위치로 회전
        // 방향값 = 이동할 목표지점 - 이동 시작 지점
        Vector3 startPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 endPos = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        transform.rotation = Quaternion.LookRotation(endPos - startPos);

        while (true)
        {
            curTime += Time.deltaTime;

            // 목표 위치에 근접하게 도달하거나 너무 오랜시간동안 "배회 샅태에 머물러 있으면
            // 새로운 시작지점과 목표지점 설정
            endPos = new Vector3(navMeshAgent.destination.x, navMeshAgent.destination.z);
            startPos = new Vector3(transform.position.x, 0, transform.position.z);
            if ((endPos - startPos).sqrMagnitude < 0.01f || curTime >= maxTime)
            {

            }

            yield return null;
        }

    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10;       // 현재 위치를 원점으로 하는 원의 반지름
        int wanderJitter = 0;        // 선택된 각도 (wanderJitterMin ~ wanderJitterMax)
        int wanderJitterMin = 0;        // 최소 각도
        int wanderJitterMax = 360;      // 최대 각도

        // 현재 적 캐릭터가 있는 월드의 중심 위치와 크기(구역을 벗어난 행동을 하지 않도록)
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        // 자신의 위치를 중심으로 반지름(wanderRadius)거리,
        // 선택된 각도(wanderJitter)에 위치한 좌표를 목표지점으로 설정
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPos = transform.position + SetAngle(wanderRadius, wanderJitter);

        //생성된 목표가 자신의 이동구역을 벗어나지 않게 조절
        targetPos.x = Mathf.Clamp(targetPos.x,
                                  rangePosition.x - rangeScale.x * 0.5f,
                                  rangePosition.x + rangeScale.x * 0.5f);
        targetPos.y = 0.0f;
        targetPos.z = Mathf.Clamp(targetPos.z,
                                  rangePosition.z - rangeScale.z * 0.5f,
                                  rangePosition.z + rangeScale.z * 0.5f);

        return targetPos;
    }

    private Vector3 SetAngle(float radius, int angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);
    }
}

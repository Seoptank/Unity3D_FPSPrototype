using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    None,
    Idle,
    Wander,
    Persuit,
    Attack,
}

public class ZombieFSM : MonoBehaviour
{
    [Header("Persuit")]
    [SerializeField]
    private float       targetRecognitionRange = 8.0f;  // 인식 범위(인식 시 => "PerSuit")
    [SerializeField]
    private float       persuitLimitRange = 10.0f;      // 추적 범위(범위 밖 => "Wander")
    
    [Header("Attack")]
    [SerializeField]
    private GameObject  colliderPrefab;                 // 콜라이더 프리팹
    [SerializeField]
    private Transform   attTransform;                   // 콜라이더 프리팹 생성 위치
    [SerializeField]
    private float       attRange;                       // 공경 범위
    [SerializeField]
    private float       attRate;                        // 공격 속도

    private EnemyState  enemyState = EnemyState.None;
    private float       lastAttTime = 0;                // 공격 주기 계산을 위한 변수

    private Status          status;
    private NavMeshAgent    nav;
    private Transform       target;
    private EnemyMemoryPool enemyMemoryPool;

    public void Setup(Transform target, EnemyMemoryPool pool)
    {
        status                  = GetComponent<Status>();
        nav                     = GetComponent<NavMeshAgent>();
        this.target             = target;   // 공격 대상(Player)
        this.enemyMemoryPool    = pool;
    }
    private void OnEnable()
    {
        ChangeState(EnemyState.Idle);
    }
    private void OnDisable()
    {
        // 적이 비활성화될 때 현재 재생중인 상태 종료,상태 None으로 설정
        StopCoroutine(enemyState.ToString());
        enemyState = EnemyState.None;
    }

    public void ChangeState(EnemyState newState)
    {
        // 현재 재생중인 상태와 같으면 return
        if (enemyState == newState) return;

        // 이전 상태 종료
        StopCoroutine(enemyState.ToString());
        // 상태 newState로 변경
        enemyState = newState;
        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        // n초후 자동으로 배회하는 상태로 바꾸틑 함수 실행
        StartCoroutine("AutoChangeFromIdleToWander");

        while(true)
        {
            // Idle상태일때 하는 행동
            CalculateDisToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1~4초간 "Idle"상태
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        ChangeState(EnemyState.Wander);
    }
    private IEnumerator Wander()
    {
        float curTime = 0;
        float maxTime = 10;

    }

    private void CalculateDisToTargetAndSelectState()
    {
        if (target == null) return;

        // 플레이어(target)과 적의 거리 계산 => 행동 선택
        float dis = Vector3.Distance(target.position, transform.position);
        if(dis <= attRange)
        {
            ChangeState(EnemyState.Attack);
        }
        else if( dis <= targetRecognitionRange)
        {
            ChangeState(EnemyState.Pursuit);
        }
        else if( dis >= persuitLimitRange)
        {
            ChangeState(EnemyState.Wander);
        }
    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius  = 10.0f;    // 배회 반경의 반지름
        int wanderJitter    = 0;        // 선택된 각도
        int wanderJitterMin = 0;        // 최소 각도
        int wanderJitterMax = 360;      // 최대 각도

        // 현재 적 캐릭터가 있는 월드의 중심 위치와 크기
        Vector3 rangePos    = Vector3.zero;
        Vector3 rangeScale  = Vector3.one * 100.0f;

        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPos = transform.position + SetAngle(wanderRadius, wanderJitter);

        // 생성된 목표가 자신의 이동구역을 벗어나지 않도록
        targetPos.x = Mathf.Clamp(targetPos.x, rangePos.x - rangeScale.x * 0.5f, rangePos.x + rangeScale.x * 0.5f);
        targetPos.y = 0.0f;
        targetPos.z = Mathf.Clamp(targetPos.x, rangePos.z - rangeScale.z * 0.5f, rangePos.x + rangeScale.z * 0.5f);

        return targetPos;
    }

    private Vector3 SetAngle(float r, int angle)
    {
        Vector3 pos = Vector3.zero;
        pos.x = Mathf.Cos(angle) * r;
        pos.z = Mathf.Sin(angle) * r;

        return pos;
    }

}

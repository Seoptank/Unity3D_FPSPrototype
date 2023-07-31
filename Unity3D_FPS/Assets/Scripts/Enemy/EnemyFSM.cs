using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    None = -1,
    Idle = 0,
    Wander,
    Pursuit,
    Attack,
}

public class EnemyFSM : MonoBehaviour
{
    [Header("Perduit")]
    [SerializeField]
    private float           targetRecognitionRange = 8;     // 인식 범위( 범위안에 들어오면 "Pursuit" 상태로 변환)
    [SerializeField]
    private float           pursuitLimitRange = 10;         // 추적 범위( 범위 밖으로 나가면 "Wander" 상태로 변환)

    [Header("Attack")]
    [SerializeField]
    private GameObject      projectilePrefab;               // 발사체 프리팹
    [SerializeField]                                        
    private Transform       projectileSpawnPoint;           // 발사체 생성위치
    [SerializeField]                                        
    private float           attackRange = 5;                // 공격 범위( 범위 안에 들어오면 "Attack" 상태로 변환)
    [SerializeField]                                        
    private float           attaackRate = 1;                // 공격 속도

    private EnemyState      enemyState = EnemyState.None;
    private float           lastAttckTime = 0;              // 공격 주기 계산 변수

    private Status          status;
    private NavMeshAgent    navMeshAgent;
    private Transform       target;

    public void Setup(Transform target)
    {
        status       = GetComponent<Status>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        this.target = target;                               // 적의 공격 대상(Player)

        // NavMeshAgent 컴포넌트에서 회전을 업데이트하지 않도록 설정
        navMeshAgent.updateRotation = false;  
    }

    private void OnEnable()
    {
        // 적이 활성화 됭 때 적의 상태를 "대기로 설정"
        ChangeState(EnemyState.Idle);
    }

    private void OnDisable()
    {
        //적이 비활성화될 때 현재 재생중인 상태를 종료하고, 상태를 "NONE"으로 설정
        StopCoroutine(enemyState.ToString());

        enemyState = EnemyState.None;
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
            // 대기 상태일 때 하는 행동
            // 타겟과의 거리에 따라 행동 선택(배회,추격, 원거리 공격)
            CalculateDisToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1~4초 시간 대기
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        // 상태를 "배회"로 변경
        ChangeState(EnemyState.Wander);
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
        Vector3 to   = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        while (true)
        {
            curTime += Time.deltaTime;

            // 목표 위치에 근접하게 도달하거나 너무 오랜시간동안 "배회 샅태에 머물러 있으면
            // 새로운 시작지점과 목표지점 설정
            to      = new Vector3(navMeshAgent.destination.x, navMeshAgent.destination.z);
            from    = new Vector3(transform.position.x, 0, transform.position.z);
            if ((to - from).sqrMagnitude < 0.01f || curTime >= maxTime)
            {
                ChangeState(EnemyState.Idle);
            }

            CalculateDisToTargetAndSelectState();

            yield return null;
        }

    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10;        // 현재 위치를 원점으로 하는 원의 반지름
        int wanderJitter = 0;           // 선택된 각도 (wanderJitterMin ~ wanderJitterMax)
        int wanderJitterMin = 0;        // 최소 각도
        int wanderJitterMax = 360;      // 최대 각도

        // 현재 적 캐릭터가 있는 월드의 중심 위치와 크기(구역을 벗어난 행동을 하지 않도록)
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale    = Vector3.one * 100.0f;

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

    private IEnumerator Pursuit()
    {
        while(true)
        {
            // 추격일때는 뛰는 속도로 이동
            navMeshAgent.speed = status.RunSpeed;

            // 타겟의 방향으로 방향 전환
            navMeshAgent.SetDestination(target.position);

            // 타겟의 방향을 계속 주시하도록 함
            LookRotationToTarget();

            // 타겟과의 거리에 따라 행동 선택
            CalculateDisToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        // 공격중일때는 멈추도록 설정
        navMeshAgent.ResetPath();

        while(true)
        {
            LookRotationToTarget();

            CalculateDisToTargetAndSelectState();

            if (Time.time - lastAttckTime > attaackRate)
            {
                lastAttckTime = Time.deltaTime;

                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().Setup(target.position);
            }

            yield return null;
        }
    }

    private void LookRotationToTarget()
    {
        // 목표 위치
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);
        // 내 위치
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        // 바로 돌기
        transform.rotation = Quaternion.LookRotation(to - from);

        // 천천히 돌기
        // Quaternion rotation = Quaternion.LookRotation(to - from);
        // transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
    }

    private void CalculateDisToTargetAndSelectState()
    {
        if (target == null) return;

        //플레이어(Target)와 적의 거리 계산 => 행동 선택
        float dis = Vector3.Distance(target.position, transform.position);
        
        if( dis <= attackRange)
        {
            ChangeState(EnemyState.Attack);
        }
        else if(dis <= targetRecognitionRange)
        {
            ChangeState(EnemyState.Pursuit);
        }
        else if(dis >= pursuitLimitRange)
        {
            ChangeState(EnemyState.Wander);
        }
    }

    private void OnDrawGizmos()
    {
        // "배회"상태일 때 이동경로 표시 
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);
    
        // 인식 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);
    
        // 추적 범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);
   
        // 공격 범위
        Gizmos.color = new Color(0.39f,0.04f,0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
   
    }
}

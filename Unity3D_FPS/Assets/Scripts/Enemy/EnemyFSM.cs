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
    private float           targetRecognitionRange = 8;     // �ν� ����( �����ȿ� ������ "Pursuit" ���·� ��ȯ)
    [SerializeField]
    private float           pursuitLimitRange = 10;         // ���� ����( ���� ������ ������ "Wander" ���·� ��ȯ)

    [Header("Attack")]
    [SerializeField]
    private GameObject      projectilePrefab;               // �߻�ü ������
    [SerializeField]                                        
    private Transform       projectileSpawnPoint;           // �߻�ü ������ġ
    [SerializeField]                                        
    private float           attackRange = 5;                // ���� ����( ���� �ȿ� ������ "Attack" ���·� ��ȯ)
    [SerializeField]                                        
    private float           attaackRate = 1;                // ���� �ӵ�

    private EnemyState      enemyState = EnemyState.None;
    private float           lastAttckTime = 0;              // ���� �ֱ� ��� ����

    private Status          status;
    private NavMeshAgent    navMeshAgent;
    private Transform       target;

    public void Setup(Transform target)
    {
        status       = GetComponent<Status>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        this.target = target;                               // ���� ���� ���(Player)

        // NavMeshAgent ������Ʈ���� ȸ���� ������Ʈ���� �ʵ��� ����
        navMeshAgent.updateRotation = false;  
    }

    private void OnEnable()
    {
        // ���� Ȱ��ȭ �� �� ���� ���¸� "���� ����"
        ChangeState(EnemyState.Idle);
    }

    private void OnDisable()
    {
        //���� ��Ȱ��ȭ�� �� ���� ������� ���¸� �����ϰ�, ���¸� "NONE"���� ����
        StopCoroutine(enemyState.ToString());

        enemyState = EnemyState.None;
    }

    public void ChangeState(EnemyState newState)
    {
        // ���� ������� ���¿� �ٲٷ��� �ϴ� ���°� ������ �ٲ� �ʿ䰡 ���� ������ return
        if (enemyState == newState) return;

        // ������ ������̴� ���� ����
        StopCoroutine(enemyState.ToString());
        // ���� ���� ���¸� newState�� ����
        enemyState = newState;
        // ���ο� ���� ����
        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        // n�� �Ŀ� "��ȸ" ���·� �����ϴ� �ڵ� ����
        StartCoroutine("AutoChangeFromIdleToWander");

        while(true)
        {
            // ��� ������ �� �ϴ� �ൿ
            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����(��ȸ,�߰�, ���Ÿ� ����)
            CalculateDisToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1~4�� �ð� ���
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        // ���¸� "��ȸ"�� ����
        ChangeState(EnemyState.Wander);
    }

    private IEnumerator Wander()
    {
        float curTime = 0;
        float maxTime = 10;

        // �̵� �ӵ� ����
        navMeshAgent.speed = status.WalkSpeed;

        //��ǥ ��ġ ����
        navMeshAgent.SetDestination(CalculateWanderPosition());

        // ��ǥ ��ġ�� ȸ��
        // ���Ⱚ = �̵��� ��ǥ���� - �̵� ���� ����
        Vector3 to   = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        while (true)
        {
            curTime += Time.deltaTime;

            // ��ǥ ��ġ�� �����ϰ� �����ϰų� �ʹ� �����ð����� "��ȸ ���¿� �ӹ��� ������
            // ���ο� ���������� ��ǥ���� ����
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
        float wanderRadius = 10;        // ���� ��ġ�� �������� �ϴ� ���� ������
        int wanderJitter = 0;           // ���õ� ���� (wanderJitterMin ~ wanderJitterMax)
        int wanderJitterMin = 0;        // �ּ� ����
        int wanderJitterMax = 360;      // �ִ� ����

        // ���� �� ĳ���Ͱ� �ִ� ������ �߽� ��ġ�� ũ��(������ ��� �ൿ�� ���� �ʵ���)
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale    = Vector3.one * 100.0f;

        // �ڽ��� ��ġ�� �߽����� ������(wanderRadius)�Ÿ�,
        // ���õ� ����(wanderJitter)�� ��ġ�� ��ǥ�� ��ǥ�������� ����
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPos = transform.position + SetAngle(wanderRadius, wanderJitter);

        //������ ��ǥ�� �ڽ��� �̵������� ����� �ʰ� ����
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
            // �߰��϶��� �ٴ� �ӵ��� �̵�
            navMeshAgent.speed = status.RunSpeed;

            // Ÿ���� �������� ���� ��ȯ
            navMeshAgent.SetDestination(target.position);

            // Ÿ���� ������ ��� �ֽ��ϵ��� ��
            LookRotationToTarget();

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����
            CalculateDisToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        // �������϶��� ���ߵ��� ����
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
        // ��ǥ ��ġ
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);
        // �� ��ġ
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        // �ٷ� ����
        transform.rotation = Quaternion.LookRotation(to - from);

        // õõ�� ����
        // Quaternion rotation = Quaternion.LookRotation(to - from);
        // transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
    }

    private void CalculateDisToTargetAndSelectState()
    {
        if (target == null) return;

        //�÷��̾�(Target)�� ���� �Ÿ� ��� => �ൿ ����
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
        // "��ȸ"������ �� �̵���� ǥ�� 
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);
    
        // �ν� ����
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);
    
        // ���� ����
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);
   
        // ���� ����
        Gizmos.color = new Color(0.39f,0.04f,0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
   
    }
}

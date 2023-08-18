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
    private float       targetRecognitionRange = 8.0f;  // �ν� ����(�ν� �� => "PerSuit")
    [SerializeField]
    private float       persuitLimitRange = 10.0f;      // ���� ����(���� �� => "Wander")
    
    [Header("Attack")]
    [SerializeField]
    private GameObject  colliderPrefab;                 // �ݶ��̴� ������
    [SerializeField]
    private Transform   attTransform;                   // �ݶ��̴� ������ ���� ��ġ
    [SerializeField]
    private float       attRange;                       // ���� ����
    [SerializeField]
    private float       attRate;                        // ���� �ӵ�

    private EnemyState  enemyState = EnemyState.None;
    private float       lastAttTime = 0;                // ���� �ֱ� ����� ���� ����

    private Status          status;
    private NavMeshAgent    nav;
    private Transform       target;
    private EnemyMemoryPool enemyMemoryPool;

    public void Setup(Transform target, EnemyMemoryPool pool)
    {
        status                  = GetComponent<Status>();
        nav                     = GetComponent<NavMeshAgent>();
        this.target             = target;   // ���� ���(Player)
        this.enemyMemoryPool    = pool;
    }
    private void OnEnable()
    {
        ChangeState(EnemyState.Idle);
    }
    private void OnDisable()
    {
        // ���� ��Ȱ��ȭ�� �� ���� ������� ���� ����,���� None���� ����
        StopCoroutine(enemyState.ToString());
        enemyState = EnemyState.None;
    }

    public void ChangeState(EnemyState newState)
    {
        // ���� ������� ���¿� ������ return
        if (enemyState == newState) return;

        // ���� ���� ����
        StopCoroutine(enemyState.ToString());
        // ���� newState�� ����
        enemyState = newState;
        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        // n���� �ڵ����� ��ȸ�ϴ� ���·� �ٲٺz �Լ� ����
        StartCoroutine("AutoChangeFromIdleToWander");

        while(true)
        {
            // Idle�����϶� �ϴ� �ൿ
            CalculateDisToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1~4�ʰ� "Idle"����
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

        // �÷��̾�(target)�� ���� �Ÿ� ��� => �ൿ ����
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
        float wanderRadius  = 10.0f;    // ��ȸ �ݰ��� ������
        int wanderJitter    = 0;        // ���õ� ����
        int wanderJitterMin = 0;        // �ּ� ����
        int wanderJitterMax = 360;      // �ִ� ����

        // ���� �� ĳ���Ͱ� �ִ� ������ �߽� ��ġ�� ũ��
        Vector3 rangePos    = Vector3.zero;
        Vector3 rangeScale  = Vector3.one * 100.0f;

        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPos = transform.position + SetAngle(wanderRadius, wanderJitter);

        // ������ ��ǥ�� �ڽ��� �̵������� ����� �ʵ���
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

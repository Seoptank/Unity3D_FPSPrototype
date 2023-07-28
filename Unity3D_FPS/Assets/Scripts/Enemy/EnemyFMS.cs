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

        // NavMeshAgent ������Ʈ���� ȸ���� ������Ʈ���� �ʵ��� ����
        navMeshAgent.updateRotation = false;
    }

    private void OnEnable()
    {
        // ���� Ȱ��ȭ �� �� ���� ���¸� "���� ����"
        ChangeState(EnemyState.IDLE);
    }

    private void OnDisable()
    {
        //���� ��Ȱ��ȭ�� �� ���� ������� ���¸� �����ϰ�, ���¸� "NONE"���� ����
        StopCoroutine(enemyState.ToString());

        enemyState = EnemyState.NONE;
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
            // "��� ������ �� �ϴ� �ൿ"

            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1~4�� �ð� ���
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        // ���¸� "��ȸ"�� ����
        ChangeState(EnemyState.WANDER);
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
        Vector3 startPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 endPos = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        transform.rotation = Quaternion.LookRotation(endPos - startPos);

        while (true)
        {
            curTime += Time.deltaTime;

            // ��ǥ ��ġ�� �����ϰ� �����ϰų� �ʹ� �����ð����� "��ȸ ���¿� �ӹ��� ������
            // ���ο� ���������� ��ǥ���� ����
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
        float wanderRadius = 10;       // ���� ��ġ�� �������� �ϴ� ���� ������
        int wanderJitter = 0;        // ���õ� ���� (wanderJitterMin ~ wanderJitterMax)
        int wanderJitterMin = 0;        // �ּ� ����
        int wanderJitterMax = 360;      // �ִ� ����

        // ���� �� ĳ���Ͱ� �ִ� ������ �߽� ��ġ�� ũ��(������ ��� �ൿ�� ���� �ʵ���)
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);
    }
}

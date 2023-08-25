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
    //    // ���� ��Ȱ��ȭ �ɶ� ���� ���� �����ϰ�,���� None���� ����
    //    StopCoroutine(zombieState.ToString());
    //
    //    zombieState = ZombieState.None;
    //}

    //==================================================================
    // ���� ���� �Լ�
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
        // 1~4�� �ð� ���
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        // ���¸� "��ȸ"�� ����
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
    // ���� �Լ�
    //==================================================================
    private IEnumerator Idle()
    {
        // �ڵ����� "Wander"�� �����ϴ� �Լ� ����
        StartCoroutine("AutoChangeFromIdleToWander");

        while(true)
        {
            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ �����ϴ� �Լ� ����
            CalculateDisToTargetAndSelectState();

            yield return null;
        }
    }
    private IEnumerator Wander()
    {
        float curTime = 0;
        float maxTime = 10;

        // �̵� �ӵ� ����
        nav.speed = status.walkSpeed;

        // ��ǥ ��ġ ����
        nav.SetDestination(CalculateWanderPos());

        // ��ǥ ��ġ�� ȸ��
        // ���Ⱚ = �̵��� ��ǥ ���� - �̵� ���� ����
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
    // ��� �Լ�
    //==================================================================
    private Vector3 CalculateWanderPos()
    {
        float wanderR    = 10;
        int wanderJit    = 0;
        int wanderJitMin = 0;
        int wanderJitMax = 360;

        Vector3 rangePos    = Vector3.zero;
        Vector3 rangeScale  = Vector3.one * 100.0f;

        // ���õ� ����(wnaderJit)�� ��ġ�� ��ǥ�� ��ǥ�������� ����
        wanderJit = Random.Range(wanderJitMin, wanderJitMax);
        Vector3 targetPos = transform.position + SetAngle(wanderR, wanderJit);

        // ������ ����� �ʵ���
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
        // ��ǥ ��ġ
        Vector3 to = new Vector3(target.position.x, 0.0f, target.position.z);
        Vector3 from = new Vector3(transform.position.x,0.0f,transform.position.z);

        // ����
        transform.rotation = Quaternion.LookRotation(to - from);
    }

    //==================================================================
    // ����� �Լ�
    //==================================================================
    private void OnDrawGizmos()
    {
        // "��ȸ"������ �� �̵���� ǥ�� 
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, nav.destination - transform.position);

        // �ν� ����
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // ���� ����
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);
    }
}

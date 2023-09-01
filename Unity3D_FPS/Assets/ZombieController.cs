using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    
    [Header("Pursuit")]
    [SerializeField]
    private Transform           target;

    [SerializeField]
    private NavMeshAgent        nav;

    private ZombieStateManager  zombieStateManager;
    private Status              status;
    private Animator            ani;

    private void Awake()
    {
        nav                 = GetComponent<NavMeshAgent>();
        status              = GetComponent<Status>();
        ani                 = GetComponent<Animator>();
        zombieStateManager  = GetComponentInChildren<ZombieStateManager>();
    }


    private void Update()
    {
        StartCoroutine("Pursuit");
    }

    

    private IEnumerator Pursuit()
    {
        while(zombieStateManager.OnStatePursuit() == true)
        {
            // Pursuit�����϶� �̵� �ӵ� ����
            nav.speed = status.RunSpeed;
            
            // target�� ��ǥ�� ����
            nav.SetDestination(target.position);

            // target������ �ٶ󺸵���
            LookRotationToTarget();

            yield return null;
        }
    }

    private void LookRotationToTarget()
    {

        Vector3 to   = new Vector3(target.position.x, 0.0f, target.position.z);
        Vector3 from = new Vector3(transform.position.x, 0.0f, transform.position.z);

        // �ٷ� ����
        // transform.rotation = Quaternion.LookRotation(to - from);

        // õõ�� ����
        Quaternion rotation = Quaternion.LookRotation(to - from);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
    }
}

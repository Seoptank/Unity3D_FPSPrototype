using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieState : MonoBehaviour
{
    [SerializeField]
    protected float     recognitionRange = 8.0f;    // Player인식 범위
    [SerializeField]
    protected float     pursuitLimitRange = 9.0f;
    [SerializeField]
    protected float     attRange = 1.0f;
    [SerializeField]
    protected Transform target;

    protected ZombieIdle        Idle;
    protected ZombieWander      Wander;
    protected ZombiePursuit     Pursuit;
    protected ZombieAttack      Attack;

    private void Awake()
    {
        Idle = GetComponentInChildren<ZombieIdle>();
        Wander = GetComponentInChildren<ZombieWander>();
        Pursuit = GetComponentInChildren<ZombiePursuit>();
        Attack = GetComponentInChildren<ZombieAttack>();   
    }

    private void Update()
    {
        

    }

    public bool OnPursuit()
    {

        float dis = Vector3.Distance(target.position, transform.position);

        if (dis <= recognitionRange) return true;

        return false;
    }
    public bool OnWander()
    {
        float dis = Vector3.Distance(target.position, transform.position);

        if (dis >= pursuitLimitRange) return true;
        return false;
    }
    public bool OnIdle()
    {
        float dis = Vector3.Distance(target.position, transform.position);

        if (dis > recognitionRange) return true;

        return false;
    }
    public bool OnAttack()
    {
        float dis = Vector3.Distance(target.position, transform.position);

        if (dis <= attRange) return true;
        return false;
    }
}

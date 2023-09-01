using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdle : ZombieState
{
    [Header("State")]
    [SerializeField]
    private ZombiePursuit pursuit;
    [SerializeField]
    private ZombieWander  wander;
    
    private float dis;

    public override ZombieState RunCurState()
    {
        if (OnPursuit() == true)
        {
            return pursuit;
        }
        else
        {
            StartCoroutine("OnWander");
            return wander;
        }
    }

    public bool OnPursuit()
    {
        dis = Vector3.Distance(target.position, transform.position);

        if(dis<=recognitionRange)
        {
            return true;
        }
        return false;
    }

    public IEnumerator OnWander()
    {
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);
    }
    

}

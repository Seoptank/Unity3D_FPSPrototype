using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

// 플레이어 정보를 관리하는 클래스
public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();

    [Header("Walk,RunSpeed")]
    [SerializeField]
    private float   walkSpeed;
    [SerializeField]
    private float   runSpeed;

    [Header("HP")]
    [SerializeField]
    private int     maxHP = 100;
    private int     curHP;

    // 외부에서 값 확인하는 용도
    public float    WalkSpeed => walkSpeed;
    public float    RunSpeed  => runSpeed;
    public int      CurHP => curHP;
    public int      MaxHP => maxHP;

    private void Awake()
    {
        curHP = maxHP;
    }

    public bool DecreaseHP(int damage)
    {
        int previousHP = curHP;

        curHP = curHP - damage > 0 ? curHP - damage : 0;

        onHPEvent.Invoke(previousHP, curHP);

        if(curHP == 0)
        {
            return true;
        }

        return false;
    }
}

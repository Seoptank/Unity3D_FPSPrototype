using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://blog.naver.com/choish1919

[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

// 플레이어 정보를 관리하는 클래스
public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();

    [Header("Walk,RunSpeed")]
    [SerializeField]
    public float   walkSpeed;
    [SerializeField]
    public float   runSpeed;

    [Header("HP/MP")]
    public int     maxHP = 100;
    public int     curHP;
    public float   maxStamina = 100;
    public float   curStamina;

    // 외부에서 값 확인하는 용도
    public float    WalkSpeed => walkSpeed;
    public float    RunSpeed  => runSpeed;
    public int      CurHP => curHP;
    public int      MaxHP => maxHP;
    public float    CurStamina => curStamina;
    public float    MaxStamina => maxStamina;

    private void Awake()
    {
        curHP = maxHP;
        curStamina = maxStamina;
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

    public void IncreaseHP(int hp)
    {
        int previousHP = curHP;

        curHP = curHP + hp > maxHP ? maxHP : curHP + hp;

        onHPEvent.Invoke(previousHP, curHP);
    }

    public void DecreaseStamina()
    {
        if(curStamina > 0)
        {
          curStamina -= 0.05f;
        }
        if(curStamina>=maxStamina)
        {
            curStamina = maxStamina;
        }
    }

    public void IncreaseStamina()
    {
        curStamina += 0.025f;
    }
}

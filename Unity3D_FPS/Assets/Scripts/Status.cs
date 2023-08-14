using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://blog.naver.com/choish1919

[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }
public class StaminaEvent : UnityEngine.Events.UnityEvent<int, int> { }

// �÷��̾� ������ �����ϴ� Ŭ����
public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();
    [HideInInspector]
    public StaminaEvent onStaminaEvent = new StaminaEvent();

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

    // �ܺο��� �� Ȯ���ϴ� �뵵
    public float    WalkSpeed => walkSpeed;
    public float    RunSpeed  => runSpeed;
    public int      CurHP => curHP;
    public int      MaxHP => maxHP;
    public float    CurStamina => curStamina;
    public float    MaxStamina => maxStamina;

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

    public void IncreaseHP(int hp)
    {
        int previousHP = curHP;

        curHP = curHP + hp > maxHP ? maxHP : curHP + hp;

        onHPEvent.Invoke(previousHP, curHP);
    }
}

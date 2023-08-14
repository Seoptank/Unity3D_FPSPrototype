using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private Stats       stats;
    public Entity       target;     

    public float HP
    {
        set => stats.HP = Mathf.Clamp(value, 0, MaxHP);
        get => stats.HP;
    }
    public float STAMINA
    {
        set => stats.STAMINA = Mathf.Clamp(value, 0, MaxStamina);
        get => stats.STAMINA;
    }

    public abstract float MaxHP             { get; }
    public abstract float HPRecovery        { get; }
    public abstract float MaxStamina        { get; }
    public abstract float StaminaRecovery   { get; }

    protected void Setup()
    {
        HP      = MaxHP;
        STAMINA = MaxStamina;

        StartCoroutine("Recovery");
    }

    protected IEnumerator Recovery()
    {
        while(true)
        {
            if (HP < MaxHP) HP += HPRecovery;
            if (STAMINA < MaxStamina) STAMINA += StaminaRecovery;

            yield return new WaitForSeconds(1);
        }
    }

    public abstract void TakeDamage(float dam);
}

[System.Serializable]
public struct Stats
{
    [HideInInspector]
    public float HP;
    [HideInInspector]
    public float STAMINA;
    [HideInInspector]
    public float WALKSPEED;
    [HideInInspector]
    public float RUNSPEED;
}

using UnityEngine;

public abstract class InteractionObject : MonoBehaviour
{
    [Header("Interaction Object")]
    [SerializeField]
    protected int maxHP = 100;
    protected int curHP;

    private void Awake()
    {
        curHP = maxHP;
    }

    public abstract void TakeDamage(int damage);

}

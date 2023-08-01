using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBarrel : InteractionObject
{
    [Header("Destructible Barrel")]
    [SerializeField]
    private GameObject      destructiveBarrelPieces;

    private bool            isDestroyed = false;

    public override void TakeDamage(int damage)
    {
        curHP -= damage;
        
        if(curHP <= 0  && isDestroyed == false)
        {
            isDestroyed = true;

            Instantiate(destructiveBarrelPieces, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}

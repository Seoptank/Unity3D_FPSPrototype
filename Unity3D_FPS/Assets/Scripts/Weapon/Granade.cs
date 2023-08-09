using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    [Header("Explosion Prefab")]
    [SerializeField] 
    private GameObject      explosionEffectPrefab;
    [SerializeField] 
    private Vector3         explosionPatticleOffset = new Vector3(0,1,0);

    [Header("Explosion Setting")]
    [SerializeField]
    private float           explosionDelay = 3.0f;      // 폭발 지연시간
    [SerializeField]
    private float           explosionForce = 700.0f;    // 폭발 힘
    [SerializeField]
    private float           explosionRadius = 5.0f;     // 폭발 반경
    [SerializeField]

    [Header("Audio Effect")]

    private float           countDown;
    private bool            hasExploded= false;
    private new Rigidbody   rigid;

    private void Start()
    {
        countDown = explosionDelay;
    }

    private void Update()
    {
        if(!hasExploded)
        {
            countDown -= Time.deltaTime;

            if(countDown <= 0.0f)
            {
                Explode();
                hasExploded = true;
            }
        }
    }

    private void Explode()
    {
        GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position + explosionPatticleOffset, Quaternion.identity);

        Destroy(explosionEffect, 4.0f);

        // 사운드 

        // 다른 오브젝트와의 상호작용
        NearbyForceApply();

        Destroy(gameObject);
    }

    private void NearbyForceApply()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach(Collider nearbyObject in colliders)
        {
            Rigidbody rigid = nearbyObject.GetComponent<Rigidbody>();

            if(rigid != null)
            {
                rigid.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }

}

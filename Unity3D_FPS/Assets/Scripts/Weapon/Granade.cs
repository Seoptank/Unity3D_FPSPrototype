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
    private float           explosionDelay = 3.0f;      // ���� �����ð�
    [SerializeField]
    private float           explosionForce = 700.0f;    // ���� ��
    [SerializeField]
    private float           explosionRadius = 5.0f;     // ���� �ݰ�
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

        // ���� 

        // �ٸ� ������Ʈ���� ��ȣ�ۿ�
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

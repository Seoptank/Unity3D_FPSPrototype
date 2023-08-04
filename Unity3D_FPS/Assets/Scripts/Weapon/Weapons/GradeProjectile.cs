using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradeProjectile : MonoBehaviour
{
    [Header("Explosion Barrel")]
    [SerializeField]
    private GameObject          explosionPrefab;
    [SerializeField]
    private float               explosionRadius = 10.0f;
    [SerializeField]
    private float               explosionForce = 500.0f;
    [SerializeField]
    private float               throwForce= 1000.0f;

    private int                 explosionDamage;
    private new Rigidbody       rigid;

    public void Setup(int damage,Vector3 rotation)
    {
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(rotation * throwForce);

        explosionDamage = damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 폭발 이펙트 생성
        Instantiate(explosionPrefab, transform.position, transform.rotation);

        // 폭발 범위에 있는 모든 오브젝트의 Collider 정보를 받아와 폭발 효과 처리
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider hit in colliders)
        {
            PlayerController player = hit.GetComponent<PlayerController>();
            if(player != null)
            {
                player.TakeDamage((int)(explosionDamage * 0.2f));
                continue;
            }

            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if(enemy != null)
            {
                enemy.TakeDamege(explosionDamage);
                continue;
            }

            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if(interaction != null)
            {
                interaction.TakeDamage(explosionDamage);
            }

            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
            if(rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // 수류탄 오브젝트 삭제
        Destroy(gameObject);
    }

}

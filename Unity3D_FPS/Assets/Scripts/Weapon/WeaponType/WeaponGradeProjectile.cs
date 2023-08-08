using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGradeProjectile : MonoBehaviour
{
    [Header("Explosion Prefab")]
    [SerializeField]
    private GameObject          explosionPrefab;
    [SerializeField]
    private Vector3             explosionParticleOffset = new Vector3(0,1,0);


    [Header("Explosion Setting")]
    [SerializeField]
    private float               explosionRadius = 10.0f;    // 폭발 반경
    [SerializeField]
    private float               explosionForce = 500.0f;    // 폭발 힘
    //[SerializeField]
    //private float               throwForce= 1000.0f;        // 던지는 힘
    [SerializeField]
    private float               explosionDelay = 3.0f;      // 폭발 지연 시간

    [Header("Audio Effects")]

    private float countdown;
    private bool  hasExploded = false;

    private void Start()
    {
        countdown = explosionDelay;
    }

    private void Update()
    {
        if(hasExploded)
        {
            countdown -= Time.deltaTime;
            if(countdown <= 0.0f)
            {
                OnExplode();
                hasExploded = true;
            }
        }
    }

    private void OnExplode()
    {

    }

    //private int                 explosionDamage;
    //private new Rigidbody       rigid;

    //public void Setup(int damage,Vector3 rotation)
    //{
    //    rigid = GetComponent<Rigidbody>();
    //    rigid.AddForce(rotation * throwForce);
    //
    //    explosionDamage = damage;
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    // 폭발 이펙트 생성
    //    Instantiate(explosionPrefab, transform.position, transform.rotation);
    //
    //    // 폭발 범위에 있는 모든 오브젝트의 Collider 정보를 받아와 폭발 효과 처리
    //    Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
    //    foreach ( Collider hit in colliders )
    //    {
    //        // 폭발 범위에 부딪힌 오브젝트가 플레이어일 때 처리
    //        PlayerController player = hit.GetComponent<PlayerController>();
    //        if(player != null)
    //        {
    //            player.TakeDamage((int)(explosionDamage * 0.2f));
    //            continue;
    //        }
    //        // 폭발 범위에 부딪힌 오브젝트가 적 태릭터일 때 처리
    //        EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
    //        if(enemy != null)
    //        {
    //            enemy.TakeDamege(explosionDamage);
    //            continue;
    //        }
    //        // 폭발 범위에 부딪힌 오브젝트가 상호작용 오븢ㄱ트이면 TakeDamage()로 피해를 줌
    //        InteractionObject interaction = hit.GetComponent<InteractionObject>();
    //        if(interaction != null)
    //        {
    //            interaction.TakeDamage(explosionDamage);
    //        }
    //        // 중력을 가지고 있는 오브젝트이면 힘을 받아 밀려나도록
    //        Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
    //        if(rigidbody != null)
    //        {
    //            rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
    //        }
    //    }
    //
    //    // 수류탄 오브젝트 삭제
    //    Destroy(gameObject);
    //}

}

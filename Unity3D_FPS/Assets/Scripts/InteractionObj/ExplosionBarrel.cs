using System.Collections;
using UnityEngine;

public class ExplosionBarrel : InteractionObject
{
    [Header("Explosion Barrel")]
    [SerializeField]
    private GameObject      explosionPrefab;
    [SerializeField]
    private float           explosionDelayTime = 0.3f;
    [SerializeField]
    private float           explositonRadious = 10.0f;
    [SerializeField]
    private float           explositonForce = 1000.0f;

    private bool            isExplode = false;

    public override void TakeDamage(int damage)
    {
        curHP -= damage;

        if(curHP <= 0 && isExplode == false)
        {
            StartCoroutine("ExplodeBarrel");
        }
    }

    private IEnumerator ExplodeBarrel()
    {
        yield return new WaitForSeconds(explosionDelayTime);

        isExplode = true;

        // 폭발 이펙트 생성 
        Bounds bounds = GetComponent<Collider>().bounds;
        Instantiate(explosionPrefab, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), transform.rotation);
        
        // 폭발 범위에 있는 모든 오브젝트의 Collider 정보를 받아와 폭발 효과를 처리  
        Collider[] colliders = Physics.OverlapSphere(transform.position, explositonRadious);
        foreach(Collider hit in colliders)
        {
            // 폭발 범위에 부딪힌 오브젝트가 플레이어일 때 처리
            PlayerController player = hit.GetComponent<PlayerController>();
            if(player != null)
            {
                player.TakeDamage(50);
                continue;
            }

            // 폭발 범위에 부딪힌 오브젝트가 적 캐릭터일 때 처리
            EnemyFSM enemy = hit.GetComponent<EnemyFSM>();
            if(enemy != null)
            {
                enemy.TakeDamege(300);
                continue;
            }

            // 폭발 범위에 부딪힌 오브젝트가 상호작용 오브젝트면 TakeDAmage()로 피해를 줌
            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if(interaction != null)
            {
                interaction.TakeDamage(300);
            }

            // 중력을 가지고 있는 오브젝트이면 힘을 받아 날라가도록
            Rigidbody rigid = hit.GetComponent<Rigidbody>();
            if (rigid != null)
            {
                rigid.AddExplosionForce(explositonForce, transform.position, explositonRadious);
            }
        }

        Destroy(gameObject);
    }
}

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

        // ���� ����Ʈ ���� 
        Bounds bounds = GetComponent<Collider>().bounds;
        Instantiate(explosionPrefab, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), transform.rotation);
        
        // ���� ������ �ִ� ��� ������Ʈ�� Collider ������ �޾ƿ� ���� ȿ���� ó��  
        Collider[] colliders = Physics.OverlapSphere(transform.position, explositonRadious);
        foreach(Collider hit in colliders)
        {
            // ���� ������ �ε��� ������Ʈ�� �÷��̾��� �� ó��
            PlayerController player = hit.GetComponent<PlayerController>();
            if(player != null)
            {
                player.TakeDamage(50);
                continue;
            }

            // ���� ������ �ε��� ������Ʈ�� �� ĳ������ �� ó��
            EnemyFSM enemy = hit.GetComponent<EnemyFSM>();
            if(enemy != null)
            {
                enemy.TakeDamege(300);
                continue;
            }

            // ���� ������ �ε��� ������Ʈ�� ��ȣ�ۿ� ������Ʈ�� TakeDAmage()�� ���ظ� ��
            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if(interaction != null)
            {
                interaction.TakeDamage(300);
            }

            // �߷��� ������ �ִ� ������Ʈ�̸� ���� �޾� ���󰡵���
            Rigidbody rigid = hit.GetComponent<Rigidbody>();
            if (rigid != null)
            {
                rigid.AddExplosionForce(explositonForce, transform.position, explositonRadious);
            }
        }

        Destroy(gameObject);
    }
}

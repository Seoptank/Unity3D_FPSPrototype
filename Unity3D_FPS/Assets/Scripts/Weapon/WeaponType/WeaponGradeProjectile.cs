using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGradeProjectile : MonoBehaviour
{
    [Header("Explosion Setting")]
    [SerializeField]
    private GameObject          explosionPrefab;
    [SerializeField]
    private float               explosionRadius = 10.0f;    // ���� �ݰ�
    [SerializeField]
    private float               explosionForce = 500.0f;    // ���� ��
    [SerializeField]
    private float               throwForce= 1000.0f;        // ������ ��

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
        // ���� ����Ʈ ����
        Instantiate(explosionPrefab, transform.position, transform.rotation);
    
        // ���� ������ �ִ� ��� ������Ʈ�� Collider ������ �޾ƿ� ���� ȿ�� ó��
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach ( Collider hit in colliders )
        {
            // ���� ������ �ε��� ������Ʈ�� �÷��̾��� �� ó��
            PlayerController player = hit.GetComponent<PlayerController>();
            if(player != null)
            {
                player.TakeDamage((int)(explosionDamage * 0.2f));
                continue;
            }
            // ���� ������ �ε��� ������Ʈ�� �� �¸����� �� ó��
            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if(enemy != null)
            {
                enemy.TakeDamege(explosionDamage);
                continue;
            }
            // ���� ������ �ε��� ������Ʈ�� ��ȣ�ۿ� ������Ʈ�̸� TakeDamage()�� ���ظ� ��
            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if(interaction != null)
            {
                interaction.TakeDamage(explosionDamage);
            }
            // �߷��� ������ �ִ� ������Ʈ�̸� ���� �޾� �з�������
            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
            if(rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    
        // ����ź ������Ʈ ����
        Destroy(gameObject);
    }

}

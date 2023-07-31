using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� �߻�ü�� �����ϴ� Ŭ����
public class EnemyProjectile : MonoBehaviour
{
    private MovementTransform   movement;
    private float               projectileDis = 30; // �߻�ü �ִ� ��Ÿ�
    private int                 damage = 5;

    public void Setup(Vector3 position)
    {
        movement = GetComponent<MovementTransform>();

        StartCoroutine("OnMove", position);
    }

    private IEnumerator OnMove(Vector3 targetPos)
    {
        Vector3 start = transform.position;

        movement.MoveTo((targetPos - transform.position).normalized);

        while(true)
        {
            if(Vector3.Distance(transform.position,start) >= projectileDis)
            {
                Destroy(gameObject);

                yield break;
            }

            yield return null;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //Debug.Log(" �÷��̾� �ǰ�");
            other.GetComponent<PlayerController>().TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}

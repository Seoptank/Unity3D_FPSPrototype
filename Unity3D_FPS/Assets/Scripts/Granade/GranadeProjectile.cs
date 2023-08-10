using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeProjectile : MonoBehaviour
{
    [Header("Explosion Prefab")]
    [SerializeField]
    private GameObject          explosionEffectPrefab;
    [SerializeField]
    private Vector3             explosionParticleOffset = new Vector3(0, 1, 0);
    [SerializeField]
    private GameObject          audioSourcePrefab;

    [Header("Explosion Setting")]
    [SerializeField]
    private float               explosionDelay = 3.0f;
    [SerializeField]
    private float               explosionForce = 700.0f;
    [SerializeField]
    private float               explosionRadius = 5.0f;
    [SerializeField]
    private int                 explosionDamage = 100;

    [Header("Explosion Audio")]
    [SerializeField]
    private AudioClip           explosionClip;
    [SerializeField]
    private AudioClip           impactSound;

    private AudioSource         audioSource;
    private float               countDown;
    private bool                hasExploded;

    public void Setup(int damage)
    {
        explosionDamage = damage;
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();    
    }

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
                OnExplode();
                hasExploded = true;
            }
        }


    }

    private void OnExplode()
    {
        GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position + explosionParticleOffset, Quaternion.identity);
        Destroy(this.gameObject);

        // ���� ����
        PlaySound(explosionClip);

        // Ÿ ������Ʈ�� ��ȣ�ۿ�
        //InteractionOtherObject();

        Destroy(explosionEffect, 2.0f);
    }

    private void PlaySound(AudioClip clip)
    {
        GameObject audioSourceObject = Instantiate(audioSourcePrefab, transform.position, Quaternion.identity);
        AudioSource instantiateAudioSource = audioSourceObject.GetComponent<AudioSource>();
        instantiateAudioSource.clip = clip;
        instantiateAudioSource.spatialBlend = 1;
        instantiateAudioSource.Play();
        Destroy(audioSourceObject, instantiateAudioSource.clip.length);
    }
    private void InteractionOtherObject()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.clip = impactSound;

        audioSource.spatialBlend = 1;

        audioSource.Play();

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            // ���� ������ �ε��� ������Ʈ�� �÷��̾��� �� ó��
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("�÷��̾�");
                player.TakeDamage((int)(explosionDamage * 0.2f));
                continue;
            }
            // ���� ������ �ε��� ������Ʈ�� �� �¸����� �� ó��
            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if (enemy != null)
            {
                Debug.Log("��");
                enemy.TakeDamege(explosionDamage);
                continue;
            }
            // ���� ������ �ε��� ������Ʈ�� ��ȣ�ۿ� ������Ʈ�̸� TakeDamage()�� ���ظ� ��
            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if (interaction != null)
            {
                Debug.Log("��ȣ�ۿ� ������Ʈ");
                interaction.TakeDamage(explosionDamage);
            }
            // �߷��� ������ �ִ� ������Ʈ�̸� ���� �޾� �з�������
            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                Debug.Log("������ٵ�");
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

}

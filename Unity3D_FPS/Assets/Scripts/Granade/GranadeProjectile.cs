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

        // 폭발 사운드
        PlaySound(explosionClip);

        // 타 오브젝트와 상호작용
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
            // 폭발 범위에 부딪힌 오브젝트가 플레이어일 때 처리
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("플레이어");
                player.TakeDamage((int)(explosionDamage * 0.2f));
                continue;
            }
            // 폭발 범위에 부딪힌 오브젝트가 적 태릭터일 때 처리
            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if (enemy != null)
            {
                Debug.Log("적");
                enemy.TakeDamege(explosionDamage);
                continue;
            }
            // 폭발 범위에 부딪힌 오브젝트가 상호작용 오븢ㄱ트이면 TakeDamage()로 피해를 줌
            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if (interaction != null)
            {
                Debug.Log("상호작용 오브젝트");
                interaction.TakeDamage(explosionDamage);
            }
            // 중력을 가지고 있는 오브젝트이면 힘을 받아 밀려나도록
            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                Debug.Log("리지드바디");
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

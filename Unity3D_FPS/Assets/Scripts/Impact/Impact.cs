using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
    private ParticleSystem      particle;
    private MemoryPool          memoryPool;
    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }
    public void Setup(MemoryPool pool)
    {
        memoryPool = pool;
    }

    private void Update()
    {
        // Particle 재생중이 아니면 삭제
        if(particle.isPlaying == false)
        {
            memoryPool.DeactivePoolItem(gameObject);
        }
    }
}

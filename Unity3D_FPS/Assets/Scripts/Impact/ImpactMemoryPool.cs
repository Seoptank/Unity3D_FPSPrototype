using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpactType
{
    NORMAL = 0,
    OBSTACLE,
    ENEMY,  
    INTERACTIONOBJECT,

}

public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[]        impactPrefab;       // 피격 이펙트
    private MemoryPool[]        memoryPool;         // 피격 이펙트 메모리풀

    private void Awake()
    {
        memoryPool = new MemoryPool[impactPrefab.Length];
        for (int i = 0; i < impactPrefab.Length; ++i)
        {
            memoryPool[i] = new MemoryPool(impactPrefab[i]);
        }
    }

    public void SpawnImpact(RaycastHit hit)
    {
        // 부딪힌 오브젝트의 Tag 정보에 따라 다르게 처리
        if(hit.transform.CompareTag("ImpactNormal"))
        {
            OnSpawnImpact(ImpactType.NORMAL, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if(hit.transform.CompareTag("ImpactObstacle"))
        {
            OnSpawnImpact(ImpactType.OBSTACLE, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if(hit.transform.CompareTag("ImpactEnemy"))
        {
            OnSpawnImpact(ImpactType.ENEMY, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if(hit.transform.CompareTag("InteractionObject"))
        {
            Color color = hit.transform.GetComponentInChildren<MeshRenderer>().material.color;
            OnSpawnImpact(ImpactType.INTERACTIONOBJECT, hit.point, Quaternion.LookRotation(hit.normal),color);
        }
    }

    public void OnSpawnImpact(ImpactType type,Vector3 position,Quaternion rotation,Color color = new Color())
    {
        GameObject item = memoryPool[(int)type].ActivePoolItem();
        item.transform.position = position;
        item.transform.rotation = rotation;
        item.GetComponent<Impact>().Setup(memoryPool[(int)type]);

        if(type == ImpactType.INTERACTIONOBJECT)
        {
            ParticleSystem.MainModule main = item.GetComponent<ParticleSystem>().main;
            main.startColor = color;
        }
    }
}

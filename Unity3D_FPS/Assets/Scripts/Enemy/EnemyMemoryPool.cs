using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private Transform       target;                                 // 적의 목표( Player )
    [SerializeField]
    private GameObject      enemySpawnPointPrefab;                  // 적이 등장하기 전 적의 등장 위치를 알려주는 프리팹
    [SerializeField]                                                
    private GameObject      enemyPrefab;                            // 적 프리팹
    [SerializeField]                                                
    private float           enemySpawnTime;                         // 적 생성 주기
    [SerializeField]                                                
    private float           enemySpawnLatency;                      // 타일 생성 후 적의 등장하기까지 대기 시간
                                                                    
    private MemoryPool      spawnPointMemoryPool;                   // 적 등장 위치를 알려주는 오브젝트 생성, 활성/비활성
    private MemoryPool      enemyMemoryPool;                        // 적 생성, 활성/비활성
                                                                    
    private int             numberOfEnemiesSpawnedAtOnc = 1;        // 동시에 생성되는 적의 숫자
    private Vector2Int      mapSize = new Vector2Int(100,100);      // 맵 크기

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool      = new MemoryPool(enemyPrefab);

        StartCoroutine("SpawnTile");
    }

    private IEnumerator SpawnTile()
    {
        int currentNumber = 0;
        int maximumNumber = 50;

        while(true)
        {
            //동시에 numberOfEnemiesSpawnedAtOnce의 숫자만큼 적이 생성
            for (int i = 0; i < numberOfEnemiesSpawnedAtOnc; ++i)
            {
                GameObject item = spawnPointMemoryPool.ActivePoolItem();

                item.transform.position = new Vector3
                                          (
                                          Random.Range(-mapSize.x * 0.49f, mapSize.x * 0.49f),
                                          1,
                                          Random.Range(-mapSize.y * 0.49f, mapSize.y * 0.49f)
                                          );
                StartCoroutine("SpawnEnemy", item);
            }

            currentNumber++;

            if(currentNumber >= maximumNumber)
            {
                currentNumber = 0;
                numberOfEnemiesSpawnedAtOnc++;
            }

            yield return new WaitForSeconds(enemySpawnTime);
        }
    }

    private IEnumerator SpawnEnemy(GameObject point)
    {
        yield return new WaitForSeconds(enemySpawnLatency);

        // 적 오브젝트를 생성하고, 적의 위치를 point의 위치로 설정
        GameObject item = enemyMemoryPool.ActivePoolItem();
        item.transform.position = point.transform.position;

        item.GetComponent<EnemyFSM>().Setup(target,this);

        // 타일 오브젝트를 비활성화
        spawnPointMemoryPool.DeactivePoolItem(point);
    }

    public void DeactivateEnemy(GameObject enemy)
    {
        enemyMemoryPool.DeactivePoolItem(enemy);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPool
{
    private class PoolItem
    {
        public bool       isActive; // "gameObject"의 활성/비활성 정보
        public GameObject gameObj;  // 화면에 보이는 실제 오브젝트
    }

    private int increaseCount = 5;      // 오브젝트 부족할 때 Instance()로 추가 생성되는 오브젝트 개수
    private int maxCount;               // 현재 리스트에 등록되어 있는 오브젝트 개수
    private int activeCount;            // 현재 게임에 활성화 되고있는 오브젝트 개수

    private GameObject      poolObj;        // 오브젝트 풀링에서 관리하는 게임 오브젝트 프리팹
    private List<PoolItem>  poolItemList;   // 관리되는 모든 오브젝트를 저장하는 리스트

    //외부에서 관리
    public int MaxCount     => maxCount;        // 현재 리스트에 등록된 오브젝트 개수 확인을 위한 프로퍼티
    public int ActiveCount  => activeCount;     // 현재 활성화된 오브젝트 개수 확인을 위한 프로퍼티

    private Vector3 tempPosition = new Vector3(48, 1, 48);

    public MemoryPool(GameObject poolObj)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObj = poolObj;

        poolItemList = new List<PoolItem>();

        InstantiateObjects();
    }

    // increaseCount단위로 오브젝트 생성
    public void InstantiateObjects()
    {
        maxCount += increaseCount;

        for (int i = 0; i < increaseCount; ++i)
        {
            PoolItem poolItem = new PoolItem();

            poolItem.isActive = false;
            poolItem.gameObj = GameObject.Instantiate(poolObj) ;
            poolItem.gameObj.transform.position = tempPosition;
            poolItem.gameObj.SetActive(false);

            poolItemList.Add(poolItem);
        }
    }

    // 현재 관리중인(활성/ 비활성) 모든 오브젝트 삭제
    // 게임 종료나 씬이 바뀔때 한번만 실행
    public void DestroyObject()
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;

        for (int i = 0; i < count; ++i)
        {
            GameObject.Destroy(poolItemList[i].gameObj);
        }

        poolItemList.Clear();
    }

    // 리스트에 저장된 오브젝트를 활성화해서 사용
    // 현재 모든 오브젝트가 사용중이면 InstantiateObjects()로 추가생성
    public GameObject ActivePoolItem()
    {
        if (poolItemList == null) return null;

        if(maxCount == activeCount)
        {
            InstantiateObjects();
        }

        int count = poolItemList.Count;
        for (int i = 0; i < count; i++)
        {
            PoolItem poolItem = poolItemList[i];

            if(poolItem.isActive == false)
            {
                activeCount++;

                poolItem.isActive = true;
                poolItem.gameObj.SetActive(true);

                return poolItem.gameObj;
            }
        }

        return null;
    }

    // 현재 사용이 완료된 오브젝트를 비활성화 상태로 설정
    public void DeactivePoolItem(GameObject removeObj)
    {
        if (poolItemList == null || removeObj == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if(poolItem.gameObj == removeObj)
            {
                activeCount--;

                poolItem.gameObj.transform.position = tempPosition;
                poolItem.isActive = false;
                poolItem.gameObj.SetActive(false);

                return;
            }
        }
    }

    // 게임에 사용중인 모든 오브젝트를 비활성화 상태로 설정
    public void DeactiveAllPoolObject()
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObj != null && poolItem.isActive == true)
            {
                poolItem.gameObj.transform.position = tempPosition;
                poolItem.isActive = false;
                poolItem.gameObj.SetActive(false);

            }
        }

        activeCount = 0;
    }
}

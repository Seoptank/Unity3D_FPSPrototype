using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPool
{
    private class PoolItem
    {
        public bool       isActive; // "gameObject"�� Ȱ��/��Ȱ�� ����
        public GameObject gameObj;  // ȭ�鿡 ���̴� ���� ������Ʈ
    }

    private int increaseCount = 5;      // ������Ʈ ������ �� Instance()�� �߰� �����Ǵ� ������Ʈ ����
    private int maxCount;               // ���� ����Ʈ�� ��ϵǾ� �ִ� ������Ʈ ����
    private int activeCount;            // ���� ���ӿ� Ȱ��ȭ �ǰ��ִ� ������Ʈ ����

    private GameObject      poolObj;        // ������Ʈ Ǯ������ �����ϴ� ���� ������Ʈ ������
    private List<PoolItem>  poolItemList;   // �����Ǵ� ��� ������Ʈ�� �����ϴ� ����Ʈ

    //�ܺο��� ����
    public int MaxCount     => maxCount;        // ���� ����Ʈ�� ��ϵ� ������Ʈ ���� Ȯ���� ���� ������Ƽ
    public int ActiveCount  => activeCount;     // ���� Ȱ��ȭ�� ������Ʈ ���� Ȯ���� ���� ������Ƽ

    private Vector3 tempPosition = new Vector3(48, 1, 48);

    public MemoryPool(GameObject poolObj)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObj = poolObj;

        poolItemList = new List<PoolItem>();

        InstantiateObjects();
    }

    // increaseCount������ ������Ʈ ����
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

    // ���� ��������(Ȱ��/ ��Ȱ��) ��� ������Ʈ ����
    // ���� ���ᳪ ���� �ٲ� �ѹ��� ����
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

    // ����Ʈ�� ����� ������Ʈ�� Ȱ��ȭ�ؼ� ���
    // ���� ��� ������Ʈ�� ������̸� InstantiateObjects()�� �߰�����
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

    // ���� ����� �Ϸ�� ������Ʈ�� ��Ȱ��ȭ ���·� ����
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

    // ���ӿ� ������� ��� ������Ʈ�� ��Ȱ��ȭ ���·� ����
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

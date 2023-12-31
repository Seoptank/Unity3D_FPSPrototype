using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMagazine : ItemBase
{
    [SerializeField]
    private GameObject          magazineEffectPrefab;
    [SerializeField]
    private int                 increaseMagazine = 2;
    [SerializeField]
    private float               rotateSpeed = 50;

    private IEnumerator Start()
    {
        while(true)
        {
            // y축을 기준으로 회전
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

            yield return null;
        }
    }
    public override void Use(GameObject entity)
    {
        entity.GetComponentInChildren<WeaponSwitchSystem>().IncreaseMagazine(WeaponType.Main,increaseMagazine);

        Instantiate(magazineEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGranade : WeaponBase
{
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip       fireClip;               // ���� ����

    [Header("Audio Clips")]
    [SerializeField]
    private GameObject      granadePrefab;          // ����ź ������
    [SerializeField]                                
    private Transform       granadeSpawnPoint;      // ����ź ���� ��ġ
    private void OnEnable()
    {
        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ źâ ������ ����
        onMagazineEvent.Invoke(weaponSetting.curMagazine);
        //���Ⱑ Ȱ��ȭ�� �� �ش� ������ ź �� ������ ����
        onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.curMagazine);
    }

    private void Awake()
    {
        base.Setup();

        //ó�� źâ �� �ִ�� ����
        weaponSetting.curMagazine    = weaponSetting.maxMagazine;
        //ó�� ź �� �ִ�� ����
        weaponSetting.curAmmo        = weaponSetting.maxAmmo;
    }
    public override void StartWeaponAction(int type = 0)
    {
        if (type == 0 && isAttack == false && weaponSetting.curAmmo > 0)
        {
            StartCoroutine("OnAttack");
        }
    }
    public override void StopWeaponAction(int type = 0)
    {

    }
    public override void StartReload()
    {

    }

    private IEnumerator OnAttack()
    {
        isAttack = true;

        // ���� �ִϸ��̼� ���
        animator.Play("Fire", -1, 0);
        // ���� ���� ���
        PlaySound(fireClip);

        yield return new WaitForEndOfFrame();

        while (true)
        {
            if (animator.CurrentAnimationIs("Movement"))
            {
                isAttack = false;

                yield break;
            }

            yield return null;
        }
    }

    public void SpawnGranadeProjectile()
    {
        GameObject granadeClone = Instantiate(granadePrefab, granadeSpawnPoint.position, Random.rotation);
        granadeClone.GetComponent<WeaponGradeProjectile>().Setup(weaponSetting.damage, transform.parent.forward);

        weaponSetting.curAmmo--;
        onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);
    }

    public override void IncreaseMagazine(int ammo)
    {
        // ����ź�� źâ�� ���� ����, ź��(Ammo)�� ����ź ������ ����ϱ� ������ ź���� ������Ų��.
        weaponSetting.curAmmo = weaponSetting.curAmmo + ammo > weaponSetting.maxAmmo ? weaponSetting.maxAmmo : weaponSetting.curAmmo + ammo;
        onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);
    }
}

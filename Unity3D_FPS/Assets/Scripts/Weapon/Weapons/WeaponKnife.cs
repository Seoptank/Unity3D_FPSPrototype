using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKnife : WeaponBase
{
    [SerializeField]
    private KnifeCollider       knifeCollider;

    private void OnEnable()
    {
        isAttack = false;

        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ źâ ������ �����Ѵ�.
        onMagazineEvent.Invoke(weaponSetting.curMagazine);

        // ���Ⱑ Ȱ��ȭ�� �� �ش� ź �� ������ ����
        onAmmoEvent.Invoke(weaponSetting.curAmmo,weaponSetting.maxAmmo);
    }
    private void Awake()
    {
        base.Setup();

        // ó�� ź ���� �ִ�� ����
        weaponSetting.curAmmo = weaponSetting.maxAmmo;

        // ó�� źâ ���� �ִ�� ����
        weaponSetting.curMagazine = weaponSetting.maxMagazine;
    }
    public override void StartWeaponAction(int type = 0)
    {
        if (isAttack == true) return;

        // ���� ����
        if (weaponSetting.isAutometicAttack == true)
        {
            StartCoroutine("OnAttackLoop", type);
        }
        // �ܹ� ����
        else
        {
            StartCoroutine("OnAttack", type);
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        isAttack = false;
        StopCoroutine("OnAttackLoop");
    }
    public override void StartReload()
    {

    }

    private IEnumerator OnAttackLoop(int type)
    {
        while(true)
        {
            yield return StartCoroutine("OnAttack", type);
        }
    }

    private IEnumerator OnAttack(int type)
    {
        isAttack = true;

        // ���� ��� ����(0,1)
        animator.SetFloat("attackType", type);

        // ���� �ִϸ��̼� ���
        animator.Play("Fire", -1, 0);

        yield return new WaitForEndOfFrame();

        while(true)
        {
            if(animator.CurrentAnimationIs("Movement"))
            {
                isAttack = false;
                
                yield break;
            }
            yield return null;
        }
    }

    public void StartWeaponKnifeCollider()
    {
        knifeCollider.StartCollider(weaponSetting.damage);
    }
}

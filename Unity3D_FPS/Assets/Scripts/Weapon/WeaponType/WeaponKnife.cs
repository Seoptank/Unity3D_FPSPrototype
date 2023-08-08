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

        // 무기가 활성화될 때 해당 무기의 탄창 정보를 갱신한다.
        onMagazineEvent.Invoke(weaponSetting.curMagazine);

        // 무기가 활성화될 때 해당 탄 수 정보를 갱신
        onAmmoEvent.Invoke(weaponSetting.curAmmo,weaponSetting.maxAmmo);
    }
    private void Awake()
    {
        base.Setup();

        mainCamera.fieldOfView = 60.0f;

        // 처음 탄 수는 최대로 설정
        weaponSetting.curAmmo = weaponSetting.maxAmmo;

        // 처음 탄창 수는 최대로 설정
        weaponSetting.curMagazine = weaponSetting.maxMagazine;
    }
    public override void StartWeaponAction(int type = 0)
    {
    }
    public override void StopWeaponAction(int type = 0)
    {
    }
    public override void StartReload()
    {
    }

    public void StartWeaponKnifeAction(int type = 0)
    {
        if (isAttack == true) return;

        // 연속 공격
        if (weaponSetting.isAutometicAttack == true)
        {
            StartCoroutine("OnAttackLoop", type);
        }
        // 단발 공격
        else
        {
            StartCoroutine("OnAttack", type);
        }
    }
    public void StopKnifeWeaponAction(int type = 0)
    {
        isAttack = false;
        StopCoroutine("OnAttackLoop");
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

        // 공격 모션 선택(0,1)
        animator.SetFloat("AttackType", type);

        // 공격 애니메이션 재생
        animator.Play("KnifeFire", -1, 0);

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

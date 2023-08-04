using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGranade : WeaponBase
{
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip fireClip;

    [Header("Audio Clips")]
    [SerializeField]
    private GameObject granadePrefab;
    [SerializeField]
    private Transform granadeSpawnPoint;
    private void OnEnable()
    {
        // 무기가 활성화될 때 해당 무기의 탄창 정보를 갱신
        onMagazineEvent.Invoke(weaponSetting.curMagazine);
        //무기가 활성화될 때 해당 무기의 탄 수 정보를 갱신
        onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.curMagazine);
    }

    private void Awake()
    {
        base.Setup();

        //처음 탄창 수 최대로 설정
        weaponSetting.curMagazine = weaponSetting.maxMagazine;

        //처음 탄 수 최대로 설정
        weaponSetting.curAmmo = weaponSetting.maxAmmo;
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

        // 공격 애니메이션 재생
        animator.Play("Fire", -1, 0);
        // 공격 사운드 재생
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
        granadeClone.GetComponent<GradeProjectile>().Setup(weaponSetting.damage, transform.parent.forward);

        weaponSetting.curAmmo--;
        onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);
    }
}

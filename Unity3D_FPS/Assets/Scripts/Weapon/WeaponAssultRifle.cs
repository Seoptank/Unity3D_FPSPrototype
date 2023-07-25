using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class WeaponAssultRifle : MonoBehaviour
{
    [HideInInspector]
    public AmmoEvent                    onAmmoEvent = new AmmoEvent();

    [Header("Fire Effect")]
    [SerializeField]
    private GameObject                  muzzleFlashEffect;         // 총구 이펙트(on/off) 

    [Header("Spawn Points")]
    [SerializeField]
    private Transform                   casingSpawnPoint;         // 탄피 생성 위치

    [Header("Audio Clips")]
    [SerializeField] 
    private AudioClip                   takeout;
    [SerializeField]
    private AudioClip                   fire;

    [Header("WeaponSetting")]
    [SerializeField] 
    private WeaponSetting               weaponSetting;

    private float                       lastAttackTime = 0;       //마지막 발사시간 채크용

    private AudioSource                 audioSource;
    private PlayerAnimationController   playerAni;
    private CasingMemoryPool            casingMemoryPool;

    // 외부에서 필요한 정보를 열람하기 위해 정의한 Get Property's
    public WeaponName WeaponName => weaponSetting.weaponName;

    private void Awake()
    {
        audioSource         = GetComponent<AudioSource>();
        playerAni           = GetComponentInParent<PlayerAnimationController>();
        casingMemoryPool    = GetComponent<CasingMemoryPool>();

        weaponSetting.curAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        PlaySound(takeout);
        muzzleFlashEffect.SetActive(false);

        //무기 비활성화될 때 해당 무기의 탄수 정보 갱신
        onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);
    }

    public void StartWeaponAction(int type = 0)
    {
        // 마우스 왼쪽 클릭(공격 시작)
        if (type == 0)
        {
            // 연속 공격시
            if(weaponSetting.isAutometicAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            }
            //단발공격
            else
            {
                OnAttack();
            }
        }
    }

    public void StopWeaponAction(int type = 0)
    {
        // 마우스 왼쪽 클릭(공격 종료)
        if (type == 0)
        {
            StopCoroutine("OnAttackLoop");
        }
    }

    private IEnumerator OnAttackLoop()
    {
        while(true)
        {
            OnAttack();

            yield return null;
        }
    }

    public void OnAttack()
    {
        if(Time.time - lastAttackTime > weaponSetting.attackRate)
        { 
            // 달리는 중 공격 불가
            if(playerAni.MoveSpeed > 0.5f)
            {
                return;
            }
            // 공격주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
            lastAttackTime = Time.time;

            // 탄 수가 없으면 공격 불가
            if(weaponSetting.curAmmo <= 0)
            {
                return;
            }

            weaponSetting.curAmmo--;
            onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);

            // 무기 발사 애니메이션 재생
            playerAni.Play("Fire", -1, 0);

            //총구 이펙트 재생
            StartCoroutine("OnMuzzleFlashEffect");
            
            // 공격 사운드 재생 
            PlaySound(fire);

            // 탄피 생성
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);
        }
    }

    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
    }

    private void PlaySound(AudioClip newClip)
    {
        audioSource.Stop();             // 기존 사운드 정지
        audioSource.clip = newClip;     // 클립에 새로운 클립 적용
        audioSource.Play();             // 적용한 클립 재생
    }
}

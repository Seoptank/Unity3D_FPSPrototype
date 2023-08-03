using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[System.Serializable]
//public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
//[System.Serializable]
//public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public class WeaponAssultRifle : WeaponBase
{
    [Header("Fire Effect")]
    [SerializeField]
    private GameObject                  muzzleFlashEffect;         // 총구 이펙트(on/off) 

    [Header("Spawn Points")]
    [SerializeField]
    private Transform                   casingSpawnPoint;         // 탄피 생성 위치
    [SerializeField]
    private Transform                   bulletSpawnPoint;         // 총알 생성 위치

    [Header("Audio Clips")]
    [SerializeField] 
    private AudioClip                   takeout;                  // 총기 꺼내는 사운드 
    [SerializeField]                                              
    private AudioClip                   fire;                     // 발사 사운드
    [SerializeField]                                              
    private AudioClip                   reload;                   // 재장전 사운드 

    [Header("AIm UI")]
    [SerializeField]
    private Image                       imageAim;                 // 조준 상태에 따라 Aim이미지 관리 

    private bool                        isModeChange = false;     // 모드 전환 여부 채크 
    private float                       defaultModeFOV = 60;      // 기본 FOV
    private float                       aimModeFOV = 30;          // aim모드 FOV

    private CasingMemoryPool            casingMemoryPool;
    private ImpactMemoryPool            impactMemoryPool;
    private Camera                      mainCamera;         // Ray 발사

    private void Awake()
    {
        // 기반 클래스의 초기화를 위한 Setup() 메소드 호출
        base.Setup();

        casingMemoryPool    = GetComponent<CasingMemoryPool>();
        impactMemoryPool    = GetComponent<ImpactMemoryPool>();
        mainCamera          = Camera.main;

        weaponSetting.curMagazine = weaponSetting.maxMagazine;

        weaponSetting.curAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        PlaySound(takeout);
        muzzleFlashEffect.SetActive(false);

        //무기 활성화될 때 해당 무기의 탄창 정보 갱신
        onMagazineEvent.Invoke(weaponSetting.curMagazine);

        //무기 활성화될 때 해당 무기의 탄수 정보 갱신
        onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);

        ResetVariables();
    }
    public override void StartWeaponAction(int type = 0)
    {
        // 재장전중일때 무기 액션을 할수 없다.
        if (isReload == true) return;

        // 모드 전환중이면 무기 액션을 할 수 없다.
        if (isModeChange == true) return;

        // 마우스 왼쪽 클릭(공격 시작)
        if (type == 0)
        {
            // 연속 공격시
            if(weaponSetting.isAutometicAttack == true)
            {
                isAttack = true;
                StartCoroutine("OnAttackLoop");
            }
            //단발공격
            else
            {
                OnAttack();
            }
        }

        // 마우스 오른쪽 클릭(모드 전환)
        else
        {
            // 공격 중일 때는 모든 전환을 할 수 없다.
            if (isAttack == true) return;

            animator.AimModeIs = true;
            imageAim.enabled = false;
            StartCoroutine("AimChange");
        }
    }
    public override void StopWeaponAction(int type = 0)
    {
        // 마우스 왼쪽 클릭(공격 종료)
        if (type == 0)
        {
            isAttack = false;
            StopCoroutine("OnAttackLoop");
        }
        else
        {
            animator.AimModeIs = false;
            imageAim.enabled = true;
            StartCoroutine("AimChange");
        }
    }
    public override void StartReload()
    {
        // 현재 재장전 중이면 재장전 불가능
        if (isReload == true || weaponSetting.curMagazine <= 0) return;

        // 현재 탄수가 최대일때 재장전 불가능
        if (weaponSetting.curAmmo == weaponSetting.maxAmmo) return;

        animator.AimModeIs = false;
        imageAim.enabled    = true;
        StartCoroutine("AimChange");

        // 무기 액션 도중에 'R'키를 눌러 재장전을 시도하면 무기 액션 종료 후 재장전
        StopWeaponAction();

        StartCoroutine("OnReload");
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
            if(animator.MoveSpeed > 0.5f)
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

            //playerAni.Play("Fire", -1, 0);
            // 무기 애니메이션 재생( 모드에 따라 AimFire or Fire 애니메이션 재생)
            string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);

            // 총구 이펙트 재생(default모드일때만 재생)
            if (animator.AimModeIs == false) StartCoroutine("OnMuzzleFlashEffect");

            // 공격 사운드 재생 
            PlaySound(fire);

            // 탄피 생성
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

            TwoStepRaycast();
        }
    }
    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
    }
    private IEnumerator OnReload()
    {
        isReload = true;

        //재장전 애니메이션, 사운드 재생
        animator.OnReload();
        PlaySound(reload);

        while(true)
        {
            // 사운드가 재생중이 아니고, 현재 애니메이션이 Movement면
            // 재장전 애니,사운드 종료
            if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;
                // 현재 탄창수를 1 감소시키고,바뀐탄창 정보를 Text UI에 업데이트
                weaponSetting.curMagazine--;
                onMagazineEvent.Invoke(weaponSetting.curMagazine);

                //현재 탄 수를 최대로 설정하고, 바뀐 탄수의 정보를 Text UI에 업데이트 
                weaponSetting.curAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);

                yield break;
            }

            yield return null;
        }
        
    }
    private void TwoStepRaycast()
    {
        Ray                 ray;
        RaycastHit          hit;
        Vector3             targetPoint = Vector3.zero;

        // 화면의 중앙 좌표(Aim 기준으로 Raycast 연산)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);

        //공격 사거리(attackDistance)안에 부딪히는 오브젝트가 있으면 
        // targetPoint는 광선에 부딪힌 위치
        if(Physics.Raycast(ray,out hit,weaponSetting.AttackDis))
        {
            targetPoint = hit.point;
        }

        // 공격 사거리 안에 부딪히는 오브젝트 없으면
        // targetPoint는 최대 사거리 위치
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.AttackDis;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.AttackDis, Color.red);

        // 첫번째 Raycast 연산으로 얻어진 targetPoint를 목표지점으로 설정,
        // 총구의 시작점으로 하여 Raycast 연산
        Vector3 attackDir = (targetPoint - bulletSpawnPoint.position).normalized;
        if(Physics.Raycast(bulletSpawnPoint.position,attackDir,out hit, weaponSetting.AttackDis))
        {
            impactMemoryPool.SpawnImpact(hit);

            if(hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamege(weaponSetting.damage);
            }
            else if(hit.transform.CompareTag("InteractionObject"))
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
            }
        }
        Debug.DrawRay(bulletSpawnPoint.position, attackDir * weaponSetting.AttackDis, Color.blue);
        
    }
    private IEnumerator AimChange()
    {
        float cur = 0;
        float percent = 0;
        float time = 0.35f;

        isModeChange        = true;

        float start = mainCamera.fieldOfView;
        float end   = animator.AimModeIs == true ? aimModeFOV : defaultModeFOV;

        while(percent < 1)
        {
            cur += Time.deltaTime;
            percent = cur / time;

            // Mode에 따라 카메라 시야각 변경
            mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        isModeChange = false;
    }
    private void ResetVariables()
    {
        isReload        = false;
        isAttack        = false;
        isModeChange    = false;
    }
    public void FireModeChange()
    {
        weaponSetting.isAutometicAttack = !weaponSetting.isAutometicAttack;
    }
    public void IncreaseMagazine(int magazine)
    {
        weaponSetting.curMagazine = CurMagazine + magazine > MaxMagazine ? MaxMagazine : CurMagazine + magazine;

        onMagazineEvent.Invoke(CurMagazine);
    }
}

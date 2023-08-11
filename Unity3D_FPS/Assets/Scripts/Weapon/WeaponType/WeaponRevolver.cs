using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponRevolver : WeaponBase
{
    [Header("Fire Effect")]
    [SerializeField]
    private GameObject          muzzleFlashEffect;      // 총구 이펙트 (On/Off)

    [Header("Spawn Points")]
    [SerializeField]
    private Transform           bulletSpawnPoint;       // 총알 생성 위치
    [SerializeField]
    private Transform           casingSpawnPoint;       // 탄피 생성 위치

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip           takeout;                // 총기 꺼내는 사운드
    [SerializeField]
    private AudioClip           fireClip;               // 발사 사운드
    [SerializeField]                                    
    private AudioClip           reloadClip;             // 장전 사운드

    [Header("Recoil")]
    [SerializeField]
    private float minX, maxX, minY, maxY;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private Vector3 rot;

    [Header("Aim UI")]
    [SerializeField]
    private Image               imageAim;

    private bool                isModeChange = false;

    private CasingMemoryPool    casingMemoryPool;
    private ImpactMemoryPool    impactMemoryPool;       // 공격 효과 생성 후 활성/ 비활성 관리

    private void OnEnable()
    {
        PlaySound(takeout);
        // 총구 이펙트 오브젝트 비활성화
        muzzleFlashEffect.SetActive(false);

        // 무기 활성화될 때 해당 무기의 탄창 정보를 갱신한다.
        onMagazineEvent.Invoke(weaponSetting.curMagazine);

        // 무기가 활성화될 때 해당 무기의 탄 수 정보를 갱신한다.
        onAmmoEvent.Invoke(weaponSetting.curAmmo,weaponSetting.maxAmmo);

        ResetVariables();
    }
    private void Awake()
    {
        base.Setup();

        casingMemoryPool    = GetComponent<CasingMemoryPool>();
        impactMemoryPool    = GetComponent<ImpactMemoryPool>();

        // 처음 탄창 수는 최대로 설정
        weaponSetting.curMagazine = weaponSetting.maxMagazine;
        // 처음 탄 수는 최대로 설정
        weaponSetting.curAmmo = weaponSetting.maxAmmo;


    }

    private void Update()
    {
        rot = cameraTransform.transform.localRotation.eulerAngles;
        if (rot.x != 0 || rot.y != 0)
        {
            cameraTransform.transform.localRotation = Quaternion.Slerp(cameraTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 3);
        }
    }

    public override void StartWeaponAction(int type = 0)
    {
        // 재장전중일때 무기액션 할수 없다.
        if (isReload == true) return;

        // 마우스 우클릭
        if(type == 0 && isAttack == false && isReload == false)
        {
            OnAttack();
        }

        // 마우스 좌클릭
        else
        {
            // 공격중일 때 모드 전환 불가
            if (isAttack == true) return;

            animator.AimModeIs = true;
            imageAim.enabled = false;
            StartCoroutine("AimChange");
        }
    }
   public override void StopWeaponAction(int type = 0)
    {
        if(type == 0)
        {
            isAttack = false;
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
        // 장전중, 탄창수가 0, 탄수가 최대치이면 재장전 불가
        if (isReload == true || weaponSetting.curMagazine <= 0) return;
        if (weaponSetting.curAmmo == weaponSetting.maxAmmo) return;

        // 무기 액션 도중에 'R'키를 눌러 재장전을 시도하면 무기 액션 종료 후 재장전
        StopWeaponAction();

        StartCoroutine("OnReload");
    }
    public void OnAttack()
    {
        if (Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            // 달리는 중 공격 불가
            if (animator.MoveSpeed > 0.5f) return;

            // 공격 주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
            lastAttackTime = Time.deltaTime;

            // 탄 수가 없으면 공격 불가능
            if (weaponSetting.curAmmo <= 0) return;

            OnRecoil();

            weaponSetting.curAmmo--;
            onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);

            // 무기 애니메이션 재생( 모드에 따라 AimFire or Fire 애니메이션 재생)
            string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);

            // 총구 이팩트 재생(default모드일때만 재생)
            if (animator.AimModeIs == false) StartCoroutine("OnMuzzleFlashEffect");

            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

            // 공격 사운드 재생
            PlaySound(fireClip);

            TwoStepRayCast();
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
        Debug.Log("OnReload 코루틴 함수 실행");
        isReload = true;

        // 재장전 애니, 사운드 재생
        animator.OnReload();
        PlaySound(reloadClip);

        while(true)
        {
            // 사운드 재생중이 아니고, 현재 애니메이션이 Movement이면
            // 재장전 애니, 사운드 종료
            if(audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;

                // 현재 탄창 수를 1감소, 바뀐 탄창 정보를 TextUI에 업데이트
                weaponSetting.curMagazine--;
                onMagazineEvent.Invoke(weaponSetting.curMagazine);

                // 현재 탄창 수를 최대로 설정, 바뀐 탄창의 정보를 TextUI에 업데이트
                weaponSetting.curAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);

                yield break;
            }

            yield return null;
        }
    }
    private void TwoStepRayCast()
    {
        Ray         ray;
        RaycastHit  hit;
        Vector3     targetPoint = Vector3.zero;

        // 화면 중앙 좌표(Aim기준으로 Raycast연산)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);

        // 공격 사거리(AttackDis)안에 부딪히는 오브젝트가 있으면
        // targetPoint는 광선에 부딪히는 위치
        if(Physics.Raycast(ray,out hit,weaponSetting.AttackDis))
        {
            targetPoint = hit.point;
        }

        // 공격 사거리안에 부딪히는 오브젝트 없으면
        // targetPoint는 최대 사거리에 위치
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.AttackDis;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.AttackDis, Color.red);

        // 첫번째 Raycast연산으로 얻어진 tqrgetPoint를 목표지점으로 설정,
        // 총구의 시작접으로 하여 Raycast 연산
        Vector3 attDir = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, attDir, out hit, weaponSetting.AttackDis))
        {
            impactMemoryPool.SpawnImpact(hit);
            if (hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamege(weaponSetting.damage);
            }
            else if( hit.transform.CompareTag("InteractionObject"))
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
            }
        }
        Debug.DrawRay(bulletSpawnPoint.position, attDir * weaponSetting.AttackDis, Color.blue);
    }
    private IEnumerator AimChange()
    {
        float cur = 0;
        float percent = 0;
        float time = 0.35f;

        isModeChange = true;

        float start = mainCamera.fieldOfView;
        float end   = animator.AimModeIs == true? aimModeFOV : defaultModeFOV;
        
        while(percent<1)
        {
            cur += Time.deltaTime;
            percent = cur / time;

            // Mode에 따라 카메라 시야각 변경
            mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

            yield return null;
        }
        isModeChange = false;
    }
    public void OnRecoil()
    {
        float recX = Random.Range(minX, maxX);
        float recY = Random.Range(minY, maxY);
        cameraTransform.transform.localRotation = Quaternion.Euler(rot.x + recY, rot.y * recX, rot.z);
    }
    private void ResetVariables()
    {
        isReload        = false;
        isAttack        = false;
        isModeChange    = false;
    }

}

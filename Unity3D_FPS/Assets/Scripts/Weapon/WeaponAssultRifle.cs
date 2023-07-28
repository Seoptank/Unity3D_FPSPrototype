using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
[System.Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public class WeaponAssultRifle : MonoBehaviour
{
    [HideInInspector]
    public AmmoEvent                    onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent                onMagazineEvent = new MagazineEvent();

    [Header("Fire Effect")]
    [SerializeField]
    private GameObject                  muzzleFlashEffect;         // �ѱ� ����Ʈ(on/off) 
    [SerializeField]
    private GameObject                  bullet;                    // �Ѿ�

    [Header("Spawn Points")]
    [SerializeField]
    private Transform                   casingSpawnPoint;         // ź�� ���� ��ġ
    [SerializeField]
    private Transform                   bulletSpawnPoint;         // �Ѿ� ���� ��ġ

    [Header("Audio Clips")]
    [SerializeField] 
    private AudioClip                   takeout;                  // �ѱ� ������ ���� 
    [SerializeField]                                              
    private AudioClip                   fire;                     // �߻� ����
    [SerializeField]                                              
    private AudioClip                   reload;                   // ������ ���� 

    [Header("WeaponSetting")]
    [SerializeField] 
    private WeaponSetting               weaponSetting;

    [Header("Weapon UI")]
    [SerializeField]
    private Image                       imageAim;                 // ���� ���¿� ���� Aim�̹��� ���� 

    private float                       lastAttackTime = 0;       // ������ �߻�ð� äũ��
    private bool                        isReload = false;         // ������ äũ��
    private bool                        isAttack = false;         // ���� ���� äũ
    private bool                        isModeChange = false;     // ��� ��ȯ ���� äũ 
    private float                       curFOV;                   // ���� FOV ���� ����
    private float                       defaultModeFOV = 60;      // �⺻ FOV
    private float                       aimModeFOV = 30;          // aim��� FOV

    private AudioSource                 audioSource;
    private PlayerAnimationController   playerAni;
    private CasingMemoryPool            casingMemoryPool;
    private ImpactMemoryPool            impactMemoryPool;
    private Camera                      mainCamera;         // Ray �߻�

    // �ܺο��� �ʿ��� ������ �����ϱ� ���� ������ Get Property's
    public WeaponName WeaponName => weaponSetting.weaponName;
    public int CurMagazine => weaponSetting.curMagazine;
    public int MaxMagazine => weaponSetting.maxMagazine;

   
    
    private void Awake()
    {
        audioSource         = GetComponent<AudioSource>();
        playerAni           = GetComponentInParent<PlayerAnimationController>();
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

        //���� Ȱ��ȭ�� �� �ش� ������ źâ ���� ����
        onMagazineEvent.Invoke(weaponSetting.curMagazine);

        //���� Ȱ��ȭ�� �� �ش� ������ ź�� ���� ����
        onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);

        ResetVariables();
    }

    public void StartWeaponAction(int type = 0)
    {
        // ���������϶� ���� �׼��� �Ҽ� ����.
        if (isReload == true) return;

        // ��� ��ȯ���̸� ���� �׼��� �� �� ����.
        if (isModeChange == true) return;

        // ���콺 ���� Ŭ��(���� ����)
        if (type == 0)
        {
            // ���� ���ݽ�
            if(weaponSetting.isAutometicAttack == true)
            {
                isAttack = true;
                StartCoroutine("OnAttackLoop");
            }
            //�ܹ߰���
            else
            {
                OnAttack();
            }
        }

        // ���콺 ������ Ŭ��(��� ��ȯ)
        else
        {
            // ���� ���� ���� ��� ��ȯ�� �� �� ����.
            if (isAttack == true) return;

            playerAni.AimModeIs = true;
            imageAim.enabled = false;
            StartCoroutine("AimChange");
        }
    }

    public void StopWeaponAction(int type = 0)
    {
        // ���콺 ���� Ŭ��(���� ����)
        if (type == 0)
        {
            isAttack = false;
            StopCoroutine("OnAttackLoop");
        }
        else
        {
            playerAni.AimModeIs = false;
            imageAim.enabled = true;
            StartCoroutine("AimChange");
        }
    }

    public void StartReload()
    {
        // ���� ������ ���̸� ������ �Ұ���
        if (isReload == true || weaponSetting.curMagazine <= 0) return;

        // ���� ź���� �ִ��϶� ������ �Ұ���
        if (weaponSetting.curAmmo == weaponSetting.maxAmmo) return;

        playerAni.AimModeIs = false;
        imageAim.enabled    = true;
        StartCoroutine("AimChange");

        // ���� �׼� ���߿� 'R'Ű�� ���� �������� �õ��ϸ� ���� �׼� ���� �� ������
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
            // �޸��� �� ���� �Ұ�
            if(playerAni.MoveSpeed > 0.5f)
            {
                return;
            }
            // �����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
            lastAttackTime = Time.time;

            // ź ���� ������ ���� �Ұ�
            if(weaponSetting.curAmmo <= 0)
            {
                return;
            }

            weaponSetting.curAmmo--;
            onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);

            //playerAni.Play("Fire", -1, 0);
            // ���� �ִϸ��̼� ���( ��忡 ���� AimFire or Fire �ִϸ��̼� ���)
            string animation = playerAni.AimModeIs == true ? "AimFire" : "Fire";
            playerAni.Play(animation, -1, 0);

            // �ѱ� ����Ʈ ���(default����϶��� ���)
            if (playerAni.AimModeIs == false) StartCoroutine("OnMuzzleFlashEffect");

            //�ѱ� ����Ʈ ���
            //StartCoroutine("OnMuzzleFlashEffect");
            
            // ���� ���� ��� 
            PlaySound(fire);

            // ź�� ����
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

        //������ �ִϸ��̼�, ���� ���
        playerAni.OnReload();
        PlaySound(reload);

        while(true)
        {
            // ���尡 ������� �ƴϰ�, ���� �ִϸ��̼��� Movement��
            // ������ �ִ�,���� ����
            if (audioSource.isPlaying == false && playerAni.CurrentAnimationIs("Movement"))
            {
                isReload = false;
                // ���� źâ���� 1 ���ҽ�Ű��,�ٲ�źâ ������ Text UI�� ������Ʈ
                weaponSetting.curMagazine--;
                onMagazineEvent.Invoke(weaponSetting.curMagazine);

                //���� ź ���� �ִ�� �����ϰ�, �ٲ� ź���� ������ Text UI�� ������Ʈ 
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

        // ȭ���� �߾� ��ǥ(Aim �������� Raycast ����)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);

        //���� ��Ÿ�(attackDistance)�ȿ� �ε����� ������Ʈ�� ������ 
        // targetPoint�� ������ �ε��� ��ġ
        if(Physics.Raycast(ray,out hit,weaponSetting.AttackDis))
        {
            targetPoint = hit.point;
        }

        // ���� ��Ÿ� �ȿ� �ε����� ������Ʈ ������
        // targetPoint�� �ִ� ��Ÿ� ��ġ
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.AttackDis;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.AttackDis, Color.red);

        // ù��° Raycast �������� ����� targetPoint�� ��ǥ�������� ����,
        // �ѱ��� ���������� �Ͽ� Raycast ����
        Vector3 attackDir = (targetPoint - bulletSpawnPoint.position).normalized;

        if(Physics.Raycast(bulletSpawnPoint.position,attackDir,out hit, weaponSetting.AttackDis))
        {
            impactMemoryPool.SpawnImpact(hit);
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
        float end   = playerAni.AimModeIs == true ? aimModeFOV : defaultModeFOV;

        while(percent < 1)
        {
            cur += Time.deltaTime;
            percent = cur / time;

            // Mode�� ���� ī�޶� �þ߰� ����
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

    private void PlaySound(AudioClip newClip)
    {
        audioSource.Stop();             // ���� ���� ����
        audioSource.clip = newClip;     // Ŭ���� ���ο� Ŭ�� ����
        audioSource.Play();             // ������ Ŭ�� ���
    }

    public void FireModeChange()
    {
        weaponSetting.isAutometicAttack = !weaponSetting.isAutometicAttack;
    }
}

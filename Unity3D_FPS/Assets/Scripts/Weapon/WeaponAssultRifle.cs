using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public class WeaponAssultRifle : MonoBehaviour
{{
    [HideInInspector]
    public AmmoEvent                    onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent                onMagazineEvent = new MagazineEvent();

    [Header("Fire Effect")]
    [SerializeField]
    private GameObject                  muzzleFlashEffect;         // �ѱ� ����Ʈ(on/off) 

    [Header("Spawn Points")]
    [SerializeField]
    private Transform                   casingSpawnPoint;         // ź�� ���� ��ġ

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

    private float                       lastAttackTime = 0;       // ������ �߻�ð� äũ��
    private bool                        isReload = false;         // ������ äũ��

    private AudioSource                 audioSource;
    private PlayerAnimationController   playerAni;
    private CasingMemoryPool            casingMemoryPool;

    // �ܺο��� �ʿ��� ������ �����ϱ� ���� ������ Get Property's
    public WeaponName WeaponName => weaponSetting.weaponName;
    public int CurMagazine => weaponSetting.curMagazine;
    public int MaxMagazine => weaponSetting.maxMagazine;

    private void Awake()
    {
        audioSource         = GetComponent<AudioSource>();
        playerAni           = GetComponentInParent<PlayerAnimationController>();
        casingMemoryPool    = GetComponent<CasingMemoryPool>();

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
    }

    public void StartWeaponAction(int type = 0)
    {
        //���������϶� ���� �׼��� �Ҽ� ����.
        if (isReload == true) return;

        // ���콺 ���� Ŭ��(���� ����)
        if (type == 0)
        {
            // ���� ���ݽ�
            if(weaponSetting.isAutometicAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            }
            //�ܹ߰���
            else
            {
                OnAttack();
            }
        }
    }

    public void StopWeaponAction(int type = 0)
    {
        // ���콺 ���� Ŭ��(���� ����)
        if (type == 0)
        {
            StopCoroutine("OnAttackLoop");
        }
    }

    public void StartReload()
    {
        // ���� ������ ���̸� ������ �Ұ���
        if (isReload == true || weaponSetting.curMagazine <= 0) return;

        if (weaponSetting.curAmmo == weaponSetting.maxAmmo) return;

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

            // ���� �߻� �ִϸ��̼� ���
            playerAni.Play("Fire", -1, 0);

            //�ѱ� ����Ʈ ���
            StartCoroutine("OnMuzzleFlashEffect");
            
            // ���� ���� ��� 
            PlaySound(fire);

            // ź�� ����
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);
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

                //���� ź ���� �ִ�� �����ϰ�, �ٲ� ź���� ������ Text UI�� ������Ʈ 
                weaponSetting.curAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);

                yield break;
            }

            yield return null;
        }
        
    }

    private void PlaySound(AudioClip newClip)
    {
        audioSource.Stop();             // ���� ���� ����
        audioSource.clip = newClip;     // Ŭ���� ���ο� Ŭ�� ����
        audioSource.Play();             // ������ Ŭ�� ���
    }
}

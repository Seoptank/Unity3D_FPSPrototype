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
    private GameObject                  muzzleFlashEffect;         // �ѱ� ����Ʈ(on/off) 

    [Header("Spawn Points")]
    [SerializeField]
    private Transform                   casingSpawnPoint;         // ź�� ���� ��ġ

    [Header("Audio Clips")]
    [SerializeField] 
    private AudioClip                   takeout;
    [SerializeField]
    private AudioClip                   fire;

    [Header("WeaponSetting")]
    [SerializeField] 
    private WeaponSetting               weaponSetting;

    private float                       lastAttackTime = 0;       //������ �߻�ð� äũ��

    private AudioSource                 audioSource;
    private PlayerAnimationController   playerAni;
    private CasingMemoryPool            casingMemoryPool;

    // �ܺο��� �ʿ��� ������ �����ϱ� ���� ������ Get Property's
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

        //���� ��Ȱ��ȭ�� �� �ش� ������ ź�� ���� ����
        onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);
    }

    public void StartWeaponAction(int type = 0)
    {
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

    private void PlaySound(AudioClip newClip)
    {
        audioSource.Stop();             // ���� ���� ����
        audioSource.clip = newClip;     // Ŭ���� ���ο� Ŭ�� ����
        audioSource.Play();             // ������ Ŭ�� ���
    }
}

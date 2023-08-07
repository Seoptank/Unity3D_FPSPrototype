using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType 
{ 
    Main = 0,
    Sub,
    Melee,
    Throw,
}

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
[System.Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }


public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Base")]
    [SerializeField]
    protected WeaponType                    weaponType;               // ���� ����
    [SerializeField]                                                  
    protected WeaponSetting                 weaponSetting;            // ���� ����

    protected float                         lastAttackTime = 0;       // ������ �߻�ð� äũ
    protected bool                          isReload = false;         // ������ äũ
    protected bool                          isAttack = false;         // ������ äũ
    protected AudioSource                   audioSource;              // ���� ��� ������Ʈ
    protected PlayerAnimationController     animator;                 // �ִϸ��̼� ��� ����
    protected Camera                        mainCamera;               // Ray �߻�
    protected float                         defaultModeFOV = 60;      // �⺻ FOV
    protected float                         aimModeFOV = 30;          // aim��� FOV
    // �ܺο��� �̺�Ʈ �Լ� ����� �Ҽ� �ֵ��� public ����
    [HideInInspector]
    public AmmoEvent                        onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent                    onMagazineEvent = new MagazineEvent();

    // �ܺο��� �ʿ��� ������ �����ϱ� ���� ������ Get Property's
    public PlayerAnimationController        Animator => animator;
    public WeaponName                       WeaponName => weaponSetting.weaponName;
    public int                              CurMagazine => weaponSetting.curMagazine;
    public int                              MaxMagazine => weaponSetting.maxMagazine;

    public abstract void StartWeaponAction(int type = 0);
    public abstract void StopWeaponAction(int type = 0);
    public abstract void StartReload();

    protected void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
    protected void Setup()
    {
        audioSource     = GetComponent<AudioSource>();  
        animator        = GetComponent<PlayerAnimationController>();
        mainCamera      = Camera.main;
    }
    public virtual void IncreaseMagazine(int magazine)
    {
        weaponSetting.curMagazine = CurMagazine + magazine > MaxMagazine ? MaxMagazine : CurMagazine + magazine;
        onMagazineEvent.Invoke(CurMagazine);
    }
}


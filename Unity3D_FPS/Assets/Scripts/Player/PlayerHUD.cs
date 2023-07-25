using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private WeaponAssultRifle       weapon;                 // ���� ������ ��µǴ� ����

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI         textWeaponName;         // ���� �̸�
    [SerializeField]                                        
    private Image                   imageWeaponIcon;        // ���� ������
    [SerializeField]                                        
    private Sprite[]                spriteWeaponIcons;      // ���� �����ܿ� ���Ǵ� Sprite �迭

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI             textAmmo;               // ����/�ִ� ź�� ���

    private void Awake()
    {
        SetupWeapon();

        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
    }

    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
    }

    private void UpdateAmmoHUD(int curAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{ curAmmo}/</size>{maxAmmo}";
    }
}

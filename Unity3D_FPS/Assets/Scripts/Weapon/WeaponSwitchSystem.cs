using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitchSystem : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private PlayerHUD playerHUD;

    [SerializeField]
    private WeaponBase[] weapons;     // �������� ���� 4����

    private WeaponBase curWeapon;      // ���� ������� ����
    private WeaponBase preWeapon;      // ���� ����ߴ� ����

    private void Awake()
    {
        // ���� ���� ����� ���� ���� �������� ��� ���� �̺�Ʈ ���
        playerHUD.SetupAllWeapons(weapons);

        // ���� �������� ��� ���⸦ ������ �ʰ� ����
        for (int i = 0; i < weapons.Length; ++i)
        {
            if (weapons[i].gameObject != null)
            {
                weapons[i].gameObject.SetActive(false);
            }
        }

        SwitchingWeapon(WeaponType.Main);
    }

    private void Update()
    {
        UpdateSwitch();
    }

    private void UpdateSwitch()
    {
        if (!Input.anyKeyDown) return;

        // 1~4�� ����Ű ������ ���� ��ü
        int inputIndex = 0;
        if (int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0 && inputIndex < 5))
        {
            SwitchingWeapon((WeaponType)(inputIndex - 1));
        }
    }

    private void SwitchingWeapon(WeaponType weaponType)
    {
        // ��ü ������ ���Ⱑ ������ ����
        if (weapons[(int)weaponType] == null)
        {
            return;
        }


        // ���� ������� ���Ⱑ ������ ���� ���� ������ ����
        if (curWeapon != null)
        {
            preWeapon = curWeapon;
        }

        // ���� ��ü
        curWeapon = weapons[(int)weaponType];

        //���� ������� ����� ��ü�Ϸ��� �� �� ����
        if (curWeapon == preWeapon)
        {
            return;
        }

        // ���⸦ ����ϴ� PlayerController,PlayerHUD�� ���� ���� ���� ����
        playerController.SwitchingWeapon(curWeapon);
        playerHUD.SwitchingWeapon(curWeapon);

        // ������ ����ϴ� ���� ��Ȱ��ȭ
        if (preWeapon != null)
        {
            preWeapon.gameObject.SetActive(false);
        }

        // ���� ����ϴ� ���� Ȱ��ȭ
        curWeapon.gameObject.SetActive(true);
    }

    public void IncreaseMagazine(WeaponType weaponType, int magazine)
    {
        // �ش� ���Ⱑ �ִ��� �˻�
        if (weapons[(int)weaponType] != null)
        {
            weapons[(int)weaponType].IncreaseMagazine(magazine);
        }
    }
    public void IncreaseMagazine(int magazine)
    {
        for (int i = 0; i < weapons.Length; ++i)
        {
            if (weapons[i] != null)
            {
                weapons[i].IncreaseMagazine(magazine);
            }
        }
    }
}
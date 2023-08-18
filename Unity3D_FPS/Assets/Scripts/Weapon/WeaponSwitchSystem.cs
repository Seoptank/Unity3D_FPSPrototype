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
    private WeaponBase[] weapons;     // 소지중인 무기 2종류

    private WeaponBase curWeapon;      // 현재 사용중인 무기
    private WeaponBase preWeapon;      // 직전 사용했던 무기

    private bool       haveAutoTypeWeapon = false;

    private void Awake()
    {
        // 무기 정보 출력을 위해 현재 소지중인 모든 무기 이벤트 등록
        playerHUD.SetupAllWeapons(weapons);

        // 현재 소지중인 모든 무기를 보이지 않게 실행
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
        HaveAutoTypeWeapon();
    }

    private void UpdateSwitch()
    {
        if (!Input.anyKeyDown) return;

        // 1~4번 숫자키 누르면 무기 교체
        int inputIndex = 0;
        if (int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0 && inputIndex < 3))
        {
            SwitchingWeapon((WeaponType)(inputIndex - 1));
        }
    }

    public bool HaveAutoTypeWeapon()
    {
        if (curWeapon == weapons[0])
        {
            haveAutoTypeWeapon = true;
            return true;
        }
        else 
            haveAutoTypeWeapon = false;

        return false;
    }    

    private void SwitchingWeapon(WeaponType weaponType)
    {
        // 교체 가능한 무기가 없으면 종료
        if (weapons[(int)weaponType] == null)
        {
            return;
        }

        // 현재 사용중인 무기가 있으면 이전 무기 정보에 저장
        if (curWeapon != null)
        {
            preWeapon = curWeapon;
        }

        // 무기 교체
        curWeapon = weapons[(int)weaponType];

        //현재 사용중인 무기로 교체하려고 할 때 종료
        if (curWeapon == preWeapon)
        {
            return;
        }

        // 무기를 사용하는 PlayerController,PlayerHUD에 현재 무기 정보 전달
        playerController.SwitchingWeapon(curWeapon);
        playerHUD.SwitchingWeapon(curWeapon);

        // 이전에 사용하던 무기 비활성화
        if (preWeapon != null)
        {
            preWeapon.gameObject.SetActive(false);
        }

        // 현재 사용하는 무기 활성화
        curWeapon.gameObject.SetActive(true);
    }

    public void IncreaseMagazine(WeaponType weaponType, int magazine)
    {
        // 해당 무기가 있는지 검사
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
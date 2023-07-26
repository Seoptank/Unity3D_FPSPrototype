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
    private TextMeshProUGUI         textAmmo;               // ����/�ִ� ź�� ���

    [Header("Magazine")]
    [SerializeField]
    private GameObject              magazineUIPrefab;       // źâ UI Prefab
    [SerializeField]                                        
    private Transform               magazineParent;         // źâUI�� �θ� Panel
                                                            
    private List<GameObject>        magazineList;           // źâ UI ����Ʈ

    private void Awake()
    {
        SetupWeapon();
        SetupMagazine();

        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        weapon.onMagazineEvent.AddListener(UpdateMagazineHUD);
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

    private void SetupMagazine()
    {
        // weapon�� �ִ� źâ ������ŭ Image Icon ����,
        // magazineParent ������Ʈ �ڽ����� ��� �� ��� ��Ȱ��ȭ/����Ʈ�� ����
        magazineList = new List<GameObject>();
        for (int i = 0; i < weapon.MaxMagazine; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);
            clone.SetActive(false);

            magazineList.Add(clone);
        }

        // weapon�� ��ϵǾ��ִ� ���� źâ ������ŭ ������Ʈ Ȱ��ȭ
        for (int i = 0; i < weapon.MaxMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }

    private void UpdateMagazineHUD(int curMagazine)
    {
        // ���� ��Ȱ��ȭ�ϰ�, curMagazine������ŭ Ȱ��ȭ
        for (int i = 0; i < magazineList.Count; ++i)
        {
            magazineList[i].SetActive(false);
        }
        for (int i = 0; i < curMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }
}

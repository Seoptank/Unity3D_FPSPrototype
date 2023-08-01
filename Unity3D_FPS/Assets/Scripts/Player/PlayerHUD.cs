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
    [SerializeField]
    private Status                  status;                 // �÷��̾� ����( �̵��ӵ�, ü�� ��)

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

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI         textHP;
    [SerializeField]
    private Image                   imageBloodScreen;
    [SerializeField]
    private AnimationCurve          curveBloodScreen;

    private void Awake()
    {
        SetupWeapon();
        SetupMagazine();

        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        weapon.onMagazineEvent.AddListener(UpdateMagazineHUD);
        status.onHPEvent.AddListener(UpdateHPHUD);
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

    private void UpdateHPHUD(int previous, int cur)
    {
        textHP.text = "HP" + cur;

        // ü���� ������������ ȭ�鿡 ������ �̹����� ������� �ʵ��� return
        if (previous <= cur) return;

        if(previous - cur > 0)
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }

    private IEnumerator OnBloodScreen()
    {
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    private WeaponBase              weapon;                 // ���� ������ ��µǴ� ����
    
    [Header("Components")]
    [SerializeField]
    private Status                  status;                 // �÷��̾� ����( �̵��ӵ�, ü�� ��)
    [SerializeField]
    private GranadeThrow            granade;
    [SerializeField]
    private WeaponAssultRifle       assultRifle;
    [SerializeField]
    private WeaponSwitchSystem      weaponSwitchingSys;

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI         textWeaponName;         // ���� �̸�
    [SerializeField]                                        
    private Image                   imageWeaponIcon;        // ���� ������
    [SerializeField]                                        
    private Sprite[]                spriteWeaponIcons;      // ���� �����ܿ� ���Ǵ� Sprite �迭
    [SerializeField]
    private Vector2[]               sizeWeaponIcons;        // ���� �������� UI ũ�� �迭

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI         textAmmo;               // ����/�ִ� ź�� ���
    [SerializeField]
    private TextMeshProUGUI         textGranadeAmmo;        // ����ź ���� ź�� ���
    
    [Header("FireType")]
    [SerializeField]
    private TextMeshProUGUI         textFireType;           // �߻� ���� ���

    [Header("Magazine")]
    [SerializeField]
    private GameObject              magazineUIPrefab;       // źâ UI Prefab
    [SerializeField]                                        
    private Transform               magazineParent;         // źâUI�� �θ� Panel
    [SerializeField]
    private int                     maxMagazineCount;       // ó�� �����ϴ� �ִ� źâ �� 
                                                            
    private List<GameObject>        magazineList;           // źâ UI ����Ʈ

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI         textHP;
    [SerializeField]
    private TextMeshProUGUI         textStamina;
    [SerializeField]
    private Image                   CircleHpBar;
    [SerializeField]
    private Image                   CircleStaminaBar;
    [SerializeField]
    private Image                   imageBloodScreen;
    [SerializeField]
    private AnimationCurve          curveBloodScreen;

    private void Start()
    {
        CircleHpBar.fillAmount = 1;
    }

    private void Update()
    {
        status.onHPEvent.AddListener(UpdateHPHUD);
        UpdateStamina((int)status.curStamina);
        CircleHpBar.fillAmount      = status.curHP * 0.01f;
        CircleStaminaBar.fillAmount = status.curStamina * 0.01f;
        UpdateGranadeAmmo();
        UpdateTextFireType();
    }

    public void SetupAllWeapons(WeaponBase[] weapons)
    {
        SetupMagazine();

        for (int i = 0; i < weapons.Length; ++i)
        {
            weapons[i].onAmmoEvent.AddListener(UpdateAmmoHUD);
            weapons[i].onMagazineEvent.AddListener(UpdateMagazineHUD);
        }
    }

    private void UpdateTextFireType()
    {
        if (weaponSwitchingSys.HaveAutoTypeWeapon() == true)
        {
            if (assultRifle.OnAutoAttack() == true)
            {
                textFireType.text = "AUTO";
            }
            else
                textFireType.text = "SEMI";
        }
        else
            textFireType.text = "SEMI";
    }

    public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;

        SetupWeapon();
    }
    private void SetupWeapon()
    {

        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
        imageWeaponIcon.rectTransform.sizeDelta = sizeWeaponIcons[(int)weapon.WeaponName];
    }

    private void UpdateAmmoHUD(int curAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{ curAmmo}/</size>{maxAmmo}";
    }

    private void UpdateGranadeAmmo()
    {
        textGranadeAmmo.text = "X" + granade.curAmmo;
    }

    private void SetupMagazine()
    {
        // weapon�� �ִ� źâ ������ŭ Image Icon ����,
        // magazineParent ������Ʈ �ڽ����� ��� �� ��� ��Ȱ��ȭ/����Ʈ�� ����
        magazineList = new List<GameObject>();
        for (int i = 0; i < maxMagazineCount; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);
            clone.SetActive(false);

            magazineList.Add(clone);
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

    private void UpdateStamina(int cur)
    {
        textStamina.text = cur.ToString();
    }

    private void UpdateHPHUD(int previous, int cur)
    {
        textHP.text =  cur.ToString();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    private WeaponBase              weapon;                 // 현재 정보가 출력되는 무기
    
    [Header("Components")]
    [SerializeField]
    private Status                  status;                 // 플레이어 상태( 이동속도, 체력 등)
    [SerializeField]
    private GranadeThrow            granade;
    [SerializeField]
    private WeaponAssultRifle       assultRifle;
    [SerializeField]
    private WeaponSwitchSystem      weaponSwitchingSys;

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI         textWeaponName;         // 무기 이름
    [SerializeField]                                        
    private Image                   imageWeaponIcon;        // 무기 아이콘
    [SerializeField]                                        
    private Sprite[]                spriteWeaponIcons;      // 무기 아이콘에 사용되는 Sprite 배열
    [SerializeField]
    private Vector2[]               sizeWeaponIcons;        // 무기 아이콘의 UI 크기 배열

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI         textAmmo;               // 현재/최대 탄수 출력
    [SerializeField]
    private TextMeshProUGUI         textGranadeAmmo;        // 수류탄 현재 탄수 출력
    
    [Header("FireType")]
    [SerializeField]
    private TextMeshProUGUI         textFireType;           // 발사 유형 출력

    [Header("Magazine")]
    [SerializeField]
    private GameObject              magazineUIPrefab;       // 탄창 UI Prefab
    [SerializeField]                                        
    private Transform               magazineParent;         // 탄창UI의 부모 Panel
    [SerializeField]
    private int                     maxMagazineCount;       // 처음 생성하는 최대 탄창 수 
                                                            
    private List<GameObject>        magazineList;           // 탄창 UI 리스트

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
        // weapon의 최대 탄창 갯수만큼 Image Icon 생성,
        // magazineParent 오브젝트 자식으로 등록 후 모두 비활성화/리스트에 저장
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
        // 전부 비활성화하고, curMagazine개수만큼 활성화
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
        // 체력이 증가했을때는 화면에 빨간색 이미지를 출력하지 않도록 return
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

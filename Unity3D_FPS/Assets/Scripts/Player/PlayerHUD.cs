using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private WeaponAssultRifle       weapon;                 // 현재 정보가 출력되는 무기
    [SerializeField]
    private Status                  status;                 // 플레이어 상태( 이동속도, 체력 등)

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI         textWeaponName;         // 무기 이름
    [SerializeField]                                        
    private Image                   imageWeaponIcon;        // 무기 아이콘
    [SerializeField]                                        
    private Sprite[]                spriteWeaponIcons;      // 무기 아이콘에 사용되는 Sprite 배열

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI         textAmmo;               // 현재/최대 탄수 출력

    [Header("Magazine")]
    [SerializeField]
    private GameObject              magazineUIPrefab;       // 탄창 UI Prefab
    [SerializeField]                                        
    private Transform               magazineParent;         // 탄창UI의 부모 Panel
                                                            
    private List<GameObject>        magazineList;           // 탄창 UI 리스트

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
        // weapon의 최대 탄창 갯수만큼 Image Icon 생성,
        // magazineParent 오브젝트 자식으로 등록 후 모두 비활성화/리스트에 저장
        magazineList = new List<GameObject>();
        for (int i = 0; i < weapon.MaxMagazine; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);
            clone.SetActive(false);

            magazineList.Add(clone);
        }

        // weapon에 등록되어있는 현재 탄창 개수만큼 오브젝트 활성화
        for (int i = 0; i < weapon.MaxMagazine; ++i)
        {
            magazineList[i].SetActive(true);
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

    private void UpdateHPHUD(int previous, int cur)
    {
        textHP.text = "HP" + cur;

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

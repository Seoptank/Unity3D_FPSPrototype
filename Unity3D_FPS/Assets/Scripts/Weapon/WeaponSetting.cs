public enum WeaponName { ASSULTRIFLE =0}

[System.Serializable]
public struct WeaponSetting
{
    public WeaponName   weaponName;             // 무기 이름
    public int          curAmmo;                // 현재 탄약 수 
    public int          maxAmmo;                // 최대 탄약 수 
    public float        attackRate;             // 공격 속도
    public float        AttackDis;              // 공격 사거리
    public bool         isAutometicAttack;      // 연속 공격 여부
}
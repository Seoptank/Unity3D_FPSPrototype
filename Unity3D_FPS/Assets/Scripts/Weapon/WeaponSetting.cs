public enum WeaponName { ASSULTRIFLE =0}

[System.Serializable]
public struct WeaponSetting
{
    public WeaponName   weaponName;             // ���� �̸�
    public int          curMagazine;            // ���� źâ ��
    public int          maxMagazine;            // �ִ� źâ ��
    public int          curAmmo;                // ���� ź�� �� 
    public int          maxAmmo;                // �ִ� ź�� �� 
    public float        attackRate;             // ���� �ӵ�
    public float        AttackDis;              // ���� ��Ÿ�
    public bool         isAutometicAttack;      // ���� ���� ����
}
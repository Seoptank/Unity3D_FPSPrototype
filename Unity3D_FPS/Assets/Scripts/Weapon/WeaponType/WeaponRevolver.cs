using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponRevolver : WeaponBase
{
    [Header("Fire Effect")]
    [SerializeField]
    private GameObject          muzzleFlashEffect;      // �ѱ� ����Ʈ (On/Off)

    [Header("Spawn Points")]
    [SerializeField]
    private Transform           bulletSpawnPoint;       // �Ѿ� ���� ��ġ
    [SerializeField]
    private Transform           casingSpawnPoint;       // ź�� ���� ��ġ

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip           takeout;                // �ѱ� ������ ����
    [SerializeField]
    private AudioClip           fireClip;               // �߻� ����
    [SerializeField]                                    
    private AudioClip           reloadClip;             // ���� ����

    [Header("Recoil")]
    [SerializeField]
    private float minX, maxX, minY, maxY;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private Vector3 rot;

    [Header("Aim UI")]
    [SerializeField]
    private Image               imageAim;

    private bool                isModeChange = false;

    private CasingMemoryPool    casingMemoryPool;
    private ImpactMemoryPool    impactMemoryPool;       // ���� ȿ�� ���� �� Ȱ��/ ��Ȱ�� ����

    private void OnEnable()
    {
        PlaySound(takeout);
        // �ѱ� ����Ʈ ������Ʈ ��Ȱ��ȭ
        muzzleFlashEffect.SetActive(false);

        // ���� Ȱ��ȭ�� �� �ش� ������ źâ ������ �����Ѵ�.
        onMagazineEvent.Invoke(weaponSetting.curMagazine);

        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ ź �� ������ �����Ѵ�.
        onAmmoEvent.Invoke(weaponSetting.curAmmo,weaponSetting.maxAmmo);

        ResetVariables();
    }
    private void Awake()
    {
        base.Setup();

        casingMemoryPool    = GetComponent<CasingMemoryPool>();
        impactMemoryPool    = GetComponent<ImpactMemoryPool>();

        // ó�� źâ ���� �ִ�� ����
        weaponSetting.curMagazine = weaponSetting.maxMagazine;
        // ó�� ź ���� �ִ�� ����
        weaponSetting.curAmmo = weaponSetting.maxAmmo;


    }

    private void Update()
    {
        rot = cameraTransform.transform.localRotation.eulerAngles;
        if (rot.x != 0 || rot.y != 0)
        {
            cameraTransform.transform.localRotation = Quaternion.Slerp(cameraTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 3);
        }
    }

    public override void StartWeaponAction(int type = 0)
    {
        // ���������϶� ����׼� �Ҽ� ����.
        if (isReload == true) return;

        // ���콺 ��Ŭ��
        if(type == 0 && isAttack == false && isReload == false)
        {
            OnAttack();
        }

        // ���콺 ��Ŭ��
        else
        {
            // �������� �� ��� ��ȯ �Ұ�
            if (isAttack == true) return;

            animator.AimModeIs = true;
            imageAim.enabled = false;
            StartCoroutine("AimChange");
        }
    }
   public override void StopWeaponAction(int type = 0)
    {
        if(type == 0)
        {
            isAttack = false;
        }
        else
        {
            animator.AimModeIs = false;
            imageAim.enabled = true;
            StartCoroutine("AimChange");
        }
    }
    public override void StartReload()
    {
        // ������, źâ���� 0, ź���� �ִ�ġ�̸� ������ �Ұ�
        if (isReload == true || weaponSetting.curMagazine <= 0) return;
        if (weaponSetting.curAmmo == weaponSetting.maxAmmo) return;

        // ���� �׼� ���߿� 'R'Ű�� ���� �������� �õ��ϸ� ���� �׼� ���� �� ������
        StopWeaponAction();

        StartCoroutine("OnReload");
    }
    public void OnAttack()
    {
        if (Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            // �޸��� �� ���� �Ұ�
            if (animator.MoveSpeed > 0.5f) return;

            // ���� �ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
            lastAttackTime = Time.deltaTime;

            // ź ���� ������ ���� �Ұ���
            if (weaponSetting.curAmmo <= 0) return;

            OnRecoil();

            weaponSetting.curAmmo--;
            onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);

            // ���� �ִϸ��̼� ���( ��忡 ���� AimFire or Fire �ִϸ��̼� ���)
            string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);

            // �ѱ� ����Ʈ ���(default����϶��� ���)
            if (animator.AimModeIs == false) StartCoroutine("OnMuzzleFlashEffect");

            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

            // ���� ���� ���
            PlaySound(fireClip);

            TwoStepRayCast();
        }
    }
    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);
        
        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
    }
    private IEnumerator OnReload()
    {
        Debug.Log("OnReload �ڷ�ƾ �Լ� ����");
        isReload = true;

        // ������ �ִ�, ���� ���
        animator.OnReload();
        PlaySound(reloadClip);

        while(true)
        {
            // ���� ������� �ƴϰ�, ���� �ִϸ��̼��� Movement�̸�
            // ������ �ִ�, ���� ����
            if(audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;

                // ���� źâ ���� 1����, �ٲ� źâ ������ TextUI�� ������Ʈ
                weaponSetting.curMagazine--;
                onMagazineEvent.Invoke(weaponSetting.curMagazine);

                // ���� źâ ���� �ִ�� ����, �ٲ� źâ�� ������ TextUI�� ������Ʈ
                weaponSetting.curAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.curAmmo, weaponSetting.maxAmmo);

                yield break;
            }

            yield return null;
        }
    }
    private void TwoStepRayCast()
    {
        Ray         ray;
        RaycastHit  hit;
        Vector3     targetPoint = Vector3.zero;

        // ȭ�� �߾� ��ǥ(Aim�������� Raycast����)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);

        // ���� ��Ÿ�(AttackDis)�ȿ� �ε����� ������Ʈ�� ������
        // targetPoint�� ������ �ε����� ��ġ
        if(Physics.Raycast(ray,out hit,weaponSetting.AttackDis))
        {
            targetPoint = hit.point;
        }

        // ���� ��Ÿ��ȿ� �ε����� ������Ʈ ������
        // targetPoint�� �ִ� ��Ÿ��� ��ġ
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.AttackDis;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.AttackDis, Color.red);

        // ù��° Raycast�������� ����� tqrgetPoint�� ��ǥ�������� ����,
        // �ѱ��� ���������� �Ͽ� Raycast ����
        Vector3 attDir = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, attDir, out hit, weaponSetting.AttackDis))
        {
            impactMemoryPool.SpawnImpact(hit);
            if (hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamege(weaponSetting.damage);
            }
            else if( hit.transform.CompareTag("InteractionObject"))
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
            }
        }
        Debug.DrawRay(bulletSpawnPoint.position, attDir * weaponSetting.AttackDis, Color.blue);
    }
    private IEnumerator AimChange()
    {
        float cur = 0;
        float percent = 0;
        float time = 0.35f;

        isModeChange = true;

        float start = mainCamera.fieldOfView;
        float end   = animator.AimModeIs == true? aimModeFOV : defaultModeFOV;
        
        while(percent<1)
        {
            cur += Time.deltaTime;
            percent = cur / time;

            // Mode�� ���� ī�޶� �þ߰� ����
            mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

            yield return null;
        }
        isModeChange = false;
    }
    public void OnRecoil()
    {
        float recX = Random.Range(minX, maxX);
        float recY = Random.Range(minY, maxY);
        cameraTransform.transform.localRotation = Quaternion.Euler(rot.x + recY, rot.y * recX, rot.z);
    }
    private void ResetVariables()
    {
        isReload        = false;
        isAttack        = false;
        isModeChange    = false;
    }

}

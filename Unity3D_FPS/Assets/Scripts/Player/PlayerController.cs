using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode runKey  = KeyCode.LeftShift;                // �޸��� Ű
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;                    // ���� Ű
    [SerializeField]
    private KeyCode reloadKey = KeyCode.R;                      // ������ Ű
    [SerializeField]
    private KeyCode fireModechangeKey = KeyCode.V;              // �ܹ�/���� ��ȯ Ű
    [SerializeField]
    private KeyCode controllModeChangeKey = KeyCode.Escape;     // ���콺 ��Ʈ�� ��� ��ȯ Ű

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip walkClip;
    [SerializeField]
    private AudioClip runClip;

    private RotateToMouse                   rotateToMouse;      // ���콺 �̵����� ī�޶� ȸ��
    private PlayerMovement                  movement;           // Ű���� �Է����� �÷��̾� �̵�, ����
    private Status                          playerState;        // �÷��̾� ����
    private AudioSource                     audioSource;        // ���� ��� ����
    private WeaponBase                      weapon;             // ��� ���Ⱑ ��ӹ޴� ��� Ŭ����

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateToMouse>();
        movement      = GetComponent<PlayerMovement>();
        playerState   = GetComponent<Status>();
        audioSource   = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(controllModeChangeKey))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Input.GetKeyUp(controllModeChangeKey))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //���콺�� UI���� ������ �Ʒ� �ڵ尡 ������� �ʵ���
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

        UpdateRotate();
        UpdateMove();
        UpdateJump();
        UpdateWeaponAction();
    }

    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.RotateTo(mouseX, mouseY);
    }

    private void UpdateMove()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // �̵� �� 
        if(x != 0 || z != 0)
        {
            bool isRun = false;

            // �����̵� �߿��� �ۼ� �ְ�
            if (z > 0)
                isRun = Input.GetKey(runKey);

            movement.MoveSpeed  = isRun == true ? playerState.RunSpeed : playerState.WalkSpeed;
            weapon.Animator.MoveSpeed = isRun == true ? 1 : 0.5f;
            audioSource.clip    = isRun == true ? runClip : walkClip;

            // ����� ������� ��� �ٽ� ������� �ʵ��� isPlaying���� äũ�� ���
            if(audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }

        // ���ڸ��� ���� ����
        else
        {
            movement.MoveSpeed  = 0.0f;
            weapon.Animator.MoveSpeed = 0.0f;

            if(audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }

        movement.MoveTo(new Vector3(x, 0.0f, z));
    }
    private void UpdateJump()
    {
        if(Input.GetKeyDown(jumpKey))
        {
            movement.Jump();
        }
    }

    private void UpdateWeaponAction()
    {
        
        
        if(Input.GetMouseButtonDown(0))
        {
            weapon.StartWeaponAction();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            weapon.StopWeaponAction();
        }

        if (Input.GetMouseButtonDown(1))
        {
            weapon.StartWeaponAction(1);
        }
        if (Input.GetMouseButtonUp(1))
        {
            weapon.StopWeaponAction(1);
        }

        if(Input.GetKeyDown(reloadKey))
        {
            weapon.StartReload();
        }

        if(Input.GetKeyDown(fireModechangeKey))
        {
            weapon.GetComponent<WeaponAssultRifle>().FireModeChange();
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            int type = Random.Range(0, 1);
            weapon.GetComponent<WeaponAssultRifle>().StartKnifeAction(type);
        }
        else if(Input.GetKeyUp(KeyCode.C))
        {
            weapon.GetComponent<WeaponAssultRifle>().StopKnifeAction();
        }

    }

    public void TakeDamage(int damage)
    {
        bool isDie = playerState.DecreaseHP(damage);

        if(isDie == true)
        {
            Debug.Log("GameOver");
        }
    }

    public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;
    }
}

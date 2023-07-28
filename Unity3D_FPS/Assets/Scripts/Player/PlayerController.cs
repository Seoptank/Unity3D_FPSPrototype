using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode runKey  = KeyCode.LeftShift; // �޸��� Ű
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;     // ���� Ű
    [SerializeField]
    private KeyCode reloadKey = KeyCode.R;       // ������ Ű
    [SerializeField]
    private KeyCode fireModechangeKey = KeyCode.V;       // ������ Ű

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip walkClip;
    [SerializeField]
    private AudioClip runClip;

    private RotateToMouse               rotateToMouse;
    private PlayerMovement              movement;
    private Status                      playerState;
    private PlayerAnimationController   playerAni;
    private AudioSource                 audioSource;
    private WeaponAssultRifle           weapon;
    
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateToMouse>();
        movement      = GetComponent<PlayerMovement>();
        playerState   = GetComponent<Status>();
        playerAni     = GetComponent<PlayerAnimationController>();
        audioSource   = GetComponent<AudioSource>();
        weapon        = GetComponentInChildren<WeaponAssultRifle>();
    }

    private void Update()
    {
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
            playerAni.MoveSpeed = isRun == true ? 1 : 0.5f;
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
            playerAni.MoveSpeed = 0.0f;

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
            weapon.FireModeChange();
        }
    }
}
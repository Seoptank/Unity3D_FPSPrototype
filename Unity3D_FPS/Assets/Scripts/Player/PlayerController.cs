using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode runKey  = KeyCode.LeftShift; // 달리기 키
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;     // 점프 키
    [SerializeField]
    private KeyCode reloadKey = KeyCode.R;       // 재장전 키

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

        // 이동 중 
        if(x != 0 || z != 0)
        {
            bool isRun = false;

            // 전방이동 중에만 뛸수 있게
            if (z > 0)
                isRun = Input.GetKey(runKey);

            movement.MoveSpeed  = isRun == true ? playerState.RunSpeed : playerState.WalkSpeed;
            playerAni.MoveSpeed = isRun == true ? 1 : 0.5f;
            audioSource.clip    = isRun == true ? runClip : walkClip;

            // 오디오 재생중일 경우 다시 재생하지 않도록 isPlaying으로 채크해 재생
            if(audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }

        // 제자리에 멈춘 상태
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
        if(Input.GetKeyDown(reloadKey))
        {
            weapon.StartReload();
        }
    }
}

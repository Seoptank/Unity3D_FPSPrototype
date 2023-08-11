using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode runKey  = KeyCode.LeftShift;                // 달리기 키
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;                    // 점프 키
    [SerializeField]
    private KeyCode reloadKey = KeyCode.R;                      // 재장전 키
    [SerializeField]
    private KeyCode fireModechangeKey = KeyCode.V;              // 단발/연발 전환 키
    [SerializeField]
    private KeyCode controllModeChangeKey = KeyCode.Escape;     // 마우스 컨트롤 모드 전환 키

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip walkClip;
    [SerializeField]
    private AudioClip runClip;

    private RotateToMouse                   rotateToMouse;      // 마우스 이동으로 카메라 회전
    private PlayerMovement                  movement;           // 키보드 입력으로 플레이어 이동, 점프
    private Status                          playerState;        // 플레이어 정보
    private AudioSource                     audioSource;        // 사운드 재생 제어
    private WeaponBase                      weapon;             // 모든 무기가 상속받는 기반 클래스

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

        //마우스가 UI위에 있을때 아래 코드가 실행되지 않도록
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

        // 이동 중 
        if(x != 0 || z != 0)
        {
            bool isRun = false;

            // 전방이동 중에만 뛸수 있게
            if (z > 0)
                isRun = Input.GetKey(runKey);

            movement.MoveSpeed  = isRun == true ? playerState.RunSpeed : playerState.WalkSpeed;
            weapon.Animator.MoveSpeed = isRun == true ? 1 : 0.5f;
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

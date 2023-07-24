using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float       moveSpeed;      // 이동 속도 
    private Vector3     moveForce;      // 이동 힘

    [SerializeField]
    private float       jumpForce;      // 점프 힘 
    [SerializeField]
    private float       gravity;        // 중력 계수

    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Max(0, value); // 음수가 적영되지 않도록 제한
        get => moveSpeed;
    }

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // isGrounded로 플레이어가 바닥에 있는지 채크
        // 공중에 있으면 중력 적용
        if( !characterController.isGrounded)
        {
            moveForce.y += gravity * Time.deltaTime;
        }

        characterController.Move(moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 dir)
    {
        // 이동 방향 = 회전값 * 방향값
        dir = transform.rotation * new Vector3(dir.x, 0.0f, dir.z);
        // 이동 힘 = 방향 * 속도 
        moveForce = new Vector3(dir.x * moveSpeed, moveForce.y, dir.z * moveSpeed);
    }

    public void Jump()
    {
        if(characterController.isGrounded)
        {
            moveForce.y = jumpForce;
        }
    }
}

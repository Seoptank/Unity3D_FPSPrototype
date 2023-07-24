using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 플레이어 정보를 관리하는 클래스
public class Status : MonoBehaviour
{
    [Header("Walk,RunSpeed")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;

    // 외부에서 값 확인하는 용도
    public float WalkSpeed => walkSpeed;
    public float RunSpeed  => runSpeed;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �÷��̾� ������ �����ϴ� Ŭ����
public class Status : MonoBehaviour
{
    [Header("Walk,RunSpeed")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;

    // �ܺο��� �� Ȯ���ϴ� �뵵
    public float WalkSpeed => walkSpeed;
    public float RunSpeed  => runSpeed;
}

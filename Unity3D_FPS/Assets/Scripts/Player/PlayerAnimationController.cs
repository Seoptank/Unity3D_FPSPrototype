using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator ani;

    private void Awake()
    {
        // �÷��̾� �������� �ڽ� ������Ʈ�� Animator ������Ʈ ����
        ani = GetComponentInChildren<Animator>();
    }
    public float MoveSpeed
    {
        set => ani.SetFloat("MovementSpeed", value);
        get => ani.GetFloat("MovementSpeed");
    }

    public void Play(string stateName,int layer,float normalizedTime)
    {
        ani.Play(stateName, layer, normalizedTime);
    }
}

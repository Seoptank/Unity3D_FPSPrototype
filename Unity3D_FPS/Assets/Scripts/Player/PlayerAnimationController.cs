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

    public void OnReload()
    {
        ani.SetTrigger("OnReload");
        Debug.Log("Reload");
    }

    public bool AimModeIs
    {
        set => ani.SetBool("IsAimMode", value);
        get => ani.GetBool("IsAimMode");
    }

    public void Play(string stateName,int layer,float normalizedTime)
    {
        ani.Play(stateName, layer, normalizedTime);
    }

    public bool CurrentAnimationIs(string name)
    {
        return ani.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    public void SetFloat(string paramName,float value)
    {
        ani.SetFloat(paramName, value);
    }
}

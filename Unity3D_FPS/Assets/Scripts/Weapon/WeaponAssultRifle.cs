using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAssultRifle : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip takeout;

    private AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        PlaySound(takeout);
    }

    private void PlaySound(AudioClip newClip)
    {
        audioSource.Stop();             // ���� ���� ����
        audioSource.clip = newClip;     // Ŭ���� ���ο� Ŭ�� ����
        audioSource.Play();             // ������ Ŭ�� ���
    }
}

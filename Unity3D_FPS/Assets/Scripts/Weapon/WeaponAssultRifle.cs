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
        audioSource.Stop();             // 기존 사운드 정지
        audioSource.clip = newClip;     // 클립에 새로운 클립 적용
        audioSource.Play();             // 적용한 클립 재생
    }
}

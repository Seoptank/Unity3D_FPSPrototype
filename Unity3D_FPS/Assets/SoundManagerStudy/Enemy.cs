using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ISoundManager
{
    public AudioClip clip;
    private string Key;

    //플레이어 사운드 재생
    public void PlaySound(string _key)
    {
        SoundManager.instance.PlayBGM(_key);
    }

    void Start()
    {
        SoundManager.instance.AddsoundClip(clip);
        
        PlaySound("");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            PlaySound(Key);
    }
}

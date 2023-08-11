using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ISoundManager
{
    public AudioClip clip;

    //플레이어 사운드 재생
    public void PlaySound(string _key)
    {
        SoundManager.instance.PlayBGM(_key);
    }

    private void Start()
    {
        SoundManager.instance.AddsoundClip(clip);
        PlaySound(""); // 노래 이름 삽입
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

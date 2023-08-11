using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ISoundManager
{
    public AudioClip clip;

    //�÷��̾� ���� ���
    public void PlaySound(string _key)
    {
        SoundManager.instance.PlayBGM(_key);
    }

    private void Start()
    {
        SoundManager.instance.AddsoundClip(clip);
        PlaySound(""); // �뷡 �̸� ����
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

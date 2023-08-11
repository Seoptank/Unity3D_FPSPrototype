using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; } = null;

    public AudioSource                      audioSource;
    public Dictionary<string, AudioClip>    soundDic;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        // �ʱ�ȭ
        audioSource = gameObject.AddComponent<AudioSource>();
        soundDic = new Dictionary<string, AudioClip>();

    }
    public void PlaySound(string _key)
    {
        if(!soundDic.ContainsKey(_key))
        {
            Debug.Log("����...");
        }

        var clip = soundDic[_key];
        audioSource.clip = clip;
    }

    public void PlayBGM(string _key)
    {
        if(!soundDic.ContainsKey(_key))
        {
            Debug.Log("BGM����...");
        }

        audioSource.clip = soundDic[_key];
        audioSource.loop = true;
        audioSource.Play();
    }
    public void StopBGM(string _key)
    {

    }

    public void AddsoundClip(AudioClip _clip)
    {
        if(soundDic.ContainsKey(_clip.name))
        {
            Debug.Log("�̹� ����");
        }

        soundDic[_clip.name] = _clip;
    }

    private void Start()
    {
        // ���� �ҷ�����
    }

    void Update()
    {
        
    }
}

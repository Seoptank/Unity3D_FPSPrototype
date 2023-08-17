using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSoundManager : MonoBehaviour
{
    public AudioSource          audio;

    public void PlaySound(AudioClip clip)
    {
        audio.Stop();
        audio.clip = clip;
        audio.Play();
    }
}



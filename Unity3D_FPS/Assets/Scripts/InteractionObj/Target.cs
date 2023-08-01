using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : InteractionObject
{
    [SerializeField]
    private AudioClip       clipTrgetUp;
    [SerializeField]
    private AudioClip       clipTrgetDown;
    [SerializeField]
    private float           targetDelayTime = 3;

    private AudioSource     audioSource;
    private bool            isPossibleHit = true;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void TakeDamage(int damage)
    {
        curHP -= damage;

        if(curHP <= 0 && isPossibleHit == true)
        {
            isPossibleHit = false;

            StartCoroutine("OnTargetDown");
        }
    }

    private IEnumerator OnTargetDown()
    {
        audioSource.clip = clipTrgetDown;
        audioSource.Play();

        yield return StartCoroutine(OnAnimation(0, 50));

        StartCoroutine("OnTargetUp");
    }

    private IEnumerator OnTargetUp()
    {
        yield return new WaitForSeconds(targetDelayTime);

        audioSource.clip = clipTrgetUp;
        audioSource.Play();

        yield return StartCoroutine(OnAnimation(50, 0));

        isPossibleHit = true;
    }

    private IEnumerator OnAnimation(float start, float end)
    {
        float percent = 0;
        float current = 0;
        float time = 1;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.rotation = Quaternion.Slerp(Quaternion.Euler(start, 0, 0), Quaternion.Euler(end, 0, 0), percent);

            yield return null;
        }

    }
}

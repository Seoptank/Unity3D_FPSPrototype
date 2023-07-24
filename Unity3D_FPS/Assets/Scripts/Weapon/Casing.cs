using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour
{
    [SerializeField]
    private float       deactiveTime = 5.0f; // 탄피 등장 후 비활성화 되는 시간
    [SerializeField]
    private float       casingSpin = 1.0f;   // 탄피 회전 속력 계수
    [SerializeField]
    private AudioClip[] audioClips;          // 탄피가 부딪힐 때 재생되는 사운드

    private Rigidbody   rigid;
    private AudioSource audio;
    private MemoryPool  memorypool;

    public void Setup(MemoryPool pool, Vector3 dir)
    {
        rigid       = GetComponent<Rigidbody>();
        audio       = GetComponent<AudioSource>();
        memorypool  = pool;

        // 탄피의 이동 속도와 회전 속력 설정
        rigid.velocity          = new Vector3(dir.x, 1.0f, dir.z);
        rigid.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
                                            Random.Range(-casingSpin, casingSpin),
                                            Random.Range(-casingSpin, casingSpin));

        // 탄피 자동 비활성화 코루틴
        StartCoroutine("DeactivateAfterTime");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 여러개의 사운드 중 임의의 사운드 선택
        int index = Random.Range(0, audioClips.Length);
        audio.clip = audioClips[index];
        audio.Play();
    }

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(deactiveTime);

        memorypool.DeactivePoolItem(this.gameObject);
    }
}

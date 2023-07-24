using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour
{
    [SerializeField]
    private float       deactiveTime = 5.0f; // ź�� ���� �� ��Ȱ��ȭ �Ǵ� �ð�
    [SerializeField]
    private float       casingSpin = 1.0f;   // ź�� ȸ�� �ӷ� ���
    [SerializeField]
    private AudioClip[] audioClips;          // ź�ǰ� �ε��� �� ����Ǵ� ����

    private Rigidbody   rigid;
    private AudioSource audio;
    private MemoryPool  memorypool;

    public void Setup(MemoryPool pool, Vector3 dir)
    {
        rigid       = GetComponent<Rigidbody>();
        audio       = GetComponent<AudioSource>();
        memorypool  = pool;

        // ź���� �̵� �ӵ��� ȸ�� �ӷ� ����
        rigid.velocity          = new Vector3(dir.x, 1.0f, dir.z);
        rigid.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
                                            Random.Range(-casingSpin, casingSpin),
                                            Random.Range(-casingSpin, casingSpin));

        // ź�� �ڵ� ��Ȱ��ȭ �ڷ�ƾ
        StartCoroutine("DeactivateAfterTime");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �������� ���� �� ������ ���� ����
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

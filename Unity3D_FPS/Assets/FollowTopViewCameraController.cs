using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTopViewCameraController : MonoBehaviour
{
    [SerializeField]
    private bool        x, y, z; // 이 값이 true이면 target의 좌표,false이면 현재 좌표를 그대로 사용
    [SerializeField]
    private Transform   target;

    private void Update()
    {
        // 대상이 없으면 종료
        if (!target) return;

        transform.position = new Vector3(x ? target.position.x : transform.position.x,
                                         y ? target.position.y : transform.position.y,
                                         z ? target.position.z : transform.position.z);
    }
}

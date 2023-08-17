using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTopViewCameraController : MonoBehaviour
{
    [SerializeField]
    private bool        x, y, z; // �� ���� true�̸� target�� ��ǥ,false�̸� ���� ��ǥ�� �״�� ���
    [SerializeField]
    private Transform   target;
    [SerializeField]
    private float rotCamYAxisSpeed = 3.0f;

    private float eulerAngleY;

    private void Update()
    {
        // ����� ������ ����
        if (!target) return;

        float mouseX = Input.GetAxis("Mouse X");
        eulerAngleY += mouseX * rotCamYAxisSpeed;   // ���콺 ��/�� �̵����� ī�޶� Y�� ȸ�� 
        transform.rotation = Quaternion.Euler(90.0f, eulerAngleY, 0.0f);

        transform.position = new Vector3(x ? target.position.x : transform.position.x,
                                         y ? target.position.y : transform.position.y,
                                         z ? target.position.z : transform.position.z);
    }
}

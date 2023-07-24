using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField]
    private float rotCamXAxisSpeed = 5.0f;
    [SerializeField]
    private float rotCamYAxisSpeed = 3.0f;

    private float limMinX = -80.0f;
    private float limMaxX = 50.0f;
    private float eulerAngleX;
    private float eulerAngleY;

    public void RotateTo(float mouseX, float mouseY)
    {
        eulerAngleY += mouseX * rotCamYAxisSpeed;   // 마우스 좌/우 이동으로 카메라 Y축 회전 
        eulerAngleX -= mouseY * rotCamXAxisSpeed;   // 마우스 상/하 이동으로 카메라 X축 회전

        eulerAngleX = ClampAngle(eulerAngleX, limMinX, limMaxX);

        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0.0f);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}

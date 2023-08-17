using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MiniMapUI : MonoBehaviour
{
    [SerializeField]
    private Camera          minimapCam;
    [SerializeField]
    private float           zoomMin = 1;
    [SerializeField]
    private float           zoomMax = 30;
    [SerializeField]
    private float           zoomOneStep = 1;
    [SerializeField]
    private TextMeshProUGUI textMapName;

    private void Awake()
    {
        //�� �̸��� ���� �� �̸����� ����
        textMapName.text = SceneManager.GetActiveScene().name;
    }

    public void ZoomIn()
    {
        // ī�޶��� orthographicSize ���� ���ҽ��� ī�޶� ���̴� �繰 ũ�� Ȯ��
        minimapCam.orthographicSize = Mathf.Max(minimapCam.orthographicSize - zoomOneStep, zoomMin);
    }

    public void ZoomOut()
    {
        // ī�޶��� orthographicSize ���� ���ҽ��� ī�޶� ���̴� �繰 ũ�� ����
        minimapCam.orthographicSize = Mathf.Min(minimapCam.orthographicSize - zoomOneStep, zoomMax);
    }
}

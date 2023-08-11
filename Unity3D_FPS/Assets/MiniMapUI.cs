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
        //맵 이름을 현재 씬 이름으로 설정
        textMapName.text = SceneManager.GetActiveScene().name;
    }

    public void ZoomIn()
    {
        // 카메라의 orthographicSize 값을 감소시켜 카메라에 보이는 사물 크기 확대
        minimapCam.orthographicSize = Mathf.Max(minimapCam.orthographicSize - zoomOneStep, zoomMin);
    }

    public void ZoomOut()
    {
        // 카메라의 orthographicSize 값을 감소시켜 카메라에 보이는 사물 크기 감소
        minimapCam.orthographicSize = Mathf.Min(minimapCam.orthographicSize - zoomOneStep, zoomMax);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeController : MonoBehaviour
{
    [Header("Granade Prefab")]
    [SerializeField] 
    private GameObject      granadePrefab;

    [Header("Granade Setting")]
    [SerializeField]
    private KeyCode         throwKey = KeyCode.G;
    [SerializeField]
    private Transform       throwPos;
    [SerializeField]
    private Vector3         throwDir = new Vector3(0,1,0);

    [Header("Granade Force")]
    [SerializeField]
    private float           throwForce = 10.0f;
    [SerializeField]
    private float           maxForce = 20.0f;

    [Header("Trajectory Settings")]
    [SerializeField]
    private LineRenderer    trajectoryLine;



    private bool            isCharging = false;
    private float           chargeTime = 0.0f;
    private Camera          mainCam;


    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if(Input.GetKeyDown(throwKey))
        {
            OnThrowing();
        }
        if(isCharging)
        {
            ChargeThrow();
        }
        if(Input.GetKeyUp(throwKey))
        {
            OffThrowing();
        }
    }

    public void OnThrowing()
    {
        // 사운드 

        isCharging = true;
        chargeTime = 0.0f;

        // 궤적
        trajectoryLine.enabled = true;
    }

    public void ChargeThrow()
    {
        chargeTime += Time.deltaTime;

        // 궤적 velocity
        Vector3 granadeVel = (mainCam.transform.forward + throwDir).normalized * Mathf.Min(chargeTime,maxForce);
        ShowTrajectory(throwPos.position + throwPos.forward, granadeVel);
    }

    public void OffThrowing()
    {
        ThrowGranade(Mathf.Min(chargeTime * throwForce, maxForce));
        isCharging = false;

        trajectoryLine.enabled = false;
    }

    public void ThrowGranade(float force)
    {
        Vector3 spawnPos = throwPos.position + mainCam.transform.forward;

        GameObject granade = Instantiate(granadePrefab, spawnPos, mainCam.transform.rotation);

        Rigidbody rigid = granade.GetComponent<Rigidbody>();

        Vector3 finalThrowDir = (mainCam.transform.forward + throwDir).normalized;
        rigid.AddForce(finalThrowDir * force, ForceMode.VelocityChange);

        // 던지는 사운드
    }

    public void ShowTrajectory(Vector3 origine, Vector3 speed)
    {
        Vector3[] points = new Vector3[100];
        trajectoryLine.positionCount = points.Length;
        for (int i = 0; i < points.Length; ++i)
        {
            float time = i * 0.1f;
            points[i] = origine + speed * time + 0.5f * Physics.gravity * time * time;
        }
        trajectoryLine.SetPositions(points);
    }


}

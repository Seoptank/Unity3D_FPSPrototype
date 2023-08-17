using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeThrow : MonoBehaviour
{
    [Header("Granade Prefab")]
    [SerializeField]
    private GameObject              granadePrefab;

    [Header("Granade Setting")]
    [SerializeField]
    private Transform               granadeSpawnPoint;
    [SerializeField]
    private Vector3                 throwDir = new Vector3(0,1,0);
    [SerializeField]
    private KeyCode                 fireGranadeKey = KeyCode.G;
    [SerializeField]
    private int                     maxAmmo = 3;
    [SerializeField]
    public int                      curAmmo;
    [SerializeField]
    private int                     minAmmo = 0;

    [Header("Granade Force")]
    [SerializeField]
    private float                   throwForce = 10.0f; 
    [SerializeField]
    private float                   maxForce = 20.0f; 

    [Header("Granade Audio")]
    [SerializeField]
    private AudioClip               throwGranadeClip;
    [SerializeField]
    private AudioClip               pullPinClip;

    [Header("TrajectorySetting")]
    [SerializeField]
    private LineRenderer            trajectoryLine;

    private Camera                  mainCam;

    private bool                        isCharging = false;
    private float                       chargingTime = 0.0f;
    private PlayerAnimationController   animator;                 

    private void Awake()
    {
        mainCam     = Camera.main;
        animator = GetComponent<PlayerAnimationController>();
    }

    private void Start()
    {
        trajectoryLine.enabled = false;
        curAmmo = maxAmmo;
    }

    void Update()
    {
        if (curAmmo <= 0) return;

        if (Input.GetKeyDown(fireGranadeKey))
        {
            StartThrowing();
        }
        if (isCharging)
        {
            ChargeThrow();
        }
        if (Input.GetKeyUp(fireGranadeKey))
        {
            animator.Play("ThrowGranade", -1, 0);
        }

    }

    public void StartThrowing()
    {
        GranadeAudioManager.instance.PlayOneShot(pullPinClip,0.5f);

        

        isCharging = true;
        chargingTime = 0.0f;

        // 수류탄 궤적
        trajectoryLine.enabled = true;

        if(curAmmo<0)
        {
            trajectoryLine.enabled = false;
        }
    }
    public void ChargeThrow()
    {
        chargingTime += Time.deltaTime;

        Vector3 granadeVel = (mainCam.transform.forward + throwDir).normalized * Mathf.Min(chargingTime * throwForce, maxForce);
        ShowTrajectory(granadeSpawnPoint.position + granadeSpawnPoint.forward, granadeVel);
    }
    public void ReleaseThrow()
    {
        curAmmo--;

        if (curAmmo <= 0)
        {
            curAmmo = minAmmo;
        }
        ThrowGranade(Mathf.Min(chargingTime * throwForce, maxForce));
        isCharging = false;

        trajectoryLine.enabled = false;
    }

    private void ThrowGranade(float force)
    {
        Vector3 SpawnPosition = granadeSpawnPoint.position + mainCam.transform.forward;

        GameObject granade = Instantiate(granadePrefab, SpawnPosition, mainCam.transform.rotation);

        Rigidbody rb = granade.GetComponent<Rigidbody>();

        Vector3 finalThrowDir = (mainCam.transform.forward + throwDir).normalized;
        rb.AddForce(finalThrowDir * force, ForceMode.VelocityChange);

        //던지는 사운드
        GranadeAudioManager.instance.PlayOneShot(throwGranadeClip, 0.5f);
        //audio.PlaySound(throwGranadeClip);
    }

    private void ShowTrajectory(Vector3 origine, Vector3 speed)
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartbeatRateEffectController : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem          particleSys;
    [SerializeField]
    private PlayerController        player;

    private float                   runSpeed = 3.0f;
    private bool                    RunState = false;

    private void Start()
    {
        RunState = player.isRun;
    }
    void Update()
    {
        OnRunEffect();

        if (Input.GetKeyDown(KeyCode.L))
        {
            OnDeadEffect();
        }
    }

    private void OnRunEffect()
    {
        var main = particleSys.main;

        if (player.OnRun() == true)
        {
            main.simulationSpeed = runSpeed;
        }
        else main.simulationSpeed = 2.0f;
    }

    private void OnDeadEffect()
    {
    }
}

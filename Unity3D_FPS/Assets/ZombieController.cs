using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    [Header("State")]
    [SerializeField]
    private ZombieStateManager zombieStateManager;

    private void Update()
    {
        
    }

    private IEnumerator Pursuit()
    {
        zombieStateManager.curState
    }
}

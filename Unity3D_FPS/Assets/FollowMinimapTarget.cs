using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMinimapTarget : MonoBehaviour
{
    [SerializeField]
    private Transform       target;

    void Update()
    {
        transform.position = new Vector3(target.position.x, 3.0f, target.position.z);    
    }
}

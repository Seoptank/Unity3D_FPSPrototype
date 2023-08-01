using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMedicBag : MonoBehaviour
{
    [SerializeField]
    private GameObject      hpEffectPrefab;
    [SerializeField]
    private int             increaseHP = 50;
    [SerializeField]
    private float           moveDis = 0.2f;
    [SerializeField]
    private float           pingpongSpeed = 0.5f;
    [SerializeField]
    private float           rotateSpeed = 50;

    private IEnumerator Start()
    {
        float y = transform.position.y;

        while(true)
    }
}

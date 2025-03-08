using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RidgidbodySleep : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int sleepCountdown  = 4;
    private Rigidbody rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (sleepCountdown > 0){
            rigid.Sleep();
            sleepCountdown--;
        }
    }
}

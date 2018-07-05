using System.Collections;
using System.Collections.Generic;
using OperationTrident.Common.AI;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public Transform attactTarget;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Attack AI");
            attactTarget.GetComponent<ReactiveTarget>().OnHit(1);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("EMP");
            attactTarget.GetComponent<ReactiveTarget>().OnEMP(2f);
        }
    }
}

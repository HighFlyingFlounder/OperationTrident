using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintableObject : MonoBehaviour {

    [SerializeField]
    private string whatToHint = "^w按^yF^w与物品交互";

    [SerializeField]
    private bool usingGrammar = true;

    [SerializeField]
    private int fontSize = 12;

    public string WhatToHint
    {
        get
        {
            return whatToHint;
        }

        set
        {
            whatToHint = value;
        }
    }

    public bool UsingGrammar
    {
        get
        {
            return usingGrammar;
        }
    }

    public int FontSize
    {
        get
        {
            return fontSize;
        }
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // 销毁脚本
    public void DestroyThis()
    {
        Destroy(this);
    }
}

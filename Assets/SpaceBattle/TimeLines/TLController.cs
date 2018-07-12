using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLController : MonoBehaviour {
    public int num = 1;
    private int count;
    public GameObject[] model;

	// Use this for initialization
	void Start () {
        if (GameMgr.instance)//联机状态
            num = GameMgr.instance.player_num;//获得该局游戏总人数
        else num = 1;//单机状态
        num -= 1;
        count = model.Length;
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < count - num; i++)//
        {
            model[count - 1 - i].SetActive(false);//
        }
        enabled = false;
    }
}

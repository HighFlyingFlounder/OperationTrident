using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testSyncAI : MonoBehaviour, NetSyncInterface
{
    NetSyncController m_NetSyncController = null;
    public void Init(NetSyncController controller)
    {
        m_NetSyncController = controller;
    }

    public void RecvData(SyncData data)
    {
        //throw new System.NotImplementedException();
    }

    public SyncData SendData()
    {
        SyncData data = new SyncData();
        return data;
        //throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            try
            {
                AIController.instance.CreateAI(1, 0, "AISwopPoints");
            }
            catch (NullReferenceException ex)
            {
                Debug.Log("Exception:" + ex);
            }
        }
    }

    public void createAIImpl(int num, int type, string swopPoints)
    {
        AIController.instance.CreateAI(num, type, swopPoints);
    }
}

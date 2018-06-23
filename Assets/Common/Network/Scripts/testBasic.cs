using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testBasic
{
    private object m_obj;
	NetSyncController controller;
    public testBasic(object obj)
    {
        m_obj = obj;
		controller = (m_obj as Component).gameObject.GetComponent<NetSyncController>();
    }

	public void Sync() {
		controller.SyncVariables();
	}

	public void RPCSync(string funcname, params object[] _params) {
		controller.RPC(funcname, m_obj.GetType(), _params);	
	}


}

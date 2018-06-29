using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class AIAgent : MonoBehaviour
    {
		public ScriptableObject AIFSMAsset = null;
        protected AIState.InitParamsBase _initParams;

		AIFSM FSM {
			get {
				if(AIFSMAsset != null)
					return AIFSMAsset as AIFSM;
                Debug.Log("没有设置AIFSM");
                throw new System.NotImplementedException();
			}
		}

        // Use this for initialization
        protected void Start()
        {
			FSM.Init(this.gameObject, _initParams);
        }

        // Update is called once per frame
        protected void Update()
        {
			FSM.Update();
        }
    }
}

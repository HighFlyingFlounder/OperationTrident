using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class AIAgent : MonoBehaviour
    {
		public ScriptableObject AIFSMAsset = null;
        protected AIState.InitParamsBase _initParams;
        protected AIFSM FSM = new AIFSM();

		AIFSMData FSMData {
			get {
				if(AIFSMAsset != null)
					return AIFSMAsset as AIFSMData;
                Debug.Log("没有设置AIFSM");
                throw new System.NotImplementedException();
			}
		}

        // Use this for initialization
        protected void Start()
        {
            FSM.FSMData = FSMData;
			FSM.Init(this.gameObject, _initParams);
        }

        // Update is called once per frame
        protected void Update()
        {
			FSM.Update();
        }
    }
}

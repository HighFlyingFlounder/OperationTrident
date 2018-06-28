using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class AIAgent : MonoBehaviour
    {
		public ScriptableObject AIFSMAsset = null;

		AIFSM FSM {
			get {
				if(AIFSMAsset != null)
					return AIFSMAsset as AIFSM;
				return ScriptableObject.CreateInstance<AIFSM>();
			}
		}

        // Use this for initialization
        void Start()
        {
			FSM.Init(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
			FSM.Update();
        }
    }
}

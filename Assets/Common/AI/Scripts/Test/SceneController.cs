using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OperationTrident.Common.AI
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField]
        WanderAIAgentInitParams[] wanderAIAgentInitParams;
        [SerializeField]
        TurretAIAgentInitParams[] turrentAIAgentInitParams;

        // Use this for initialization
        void Start()
        {
            AIController.instance.CreateAI(7, 0, "AIBorn0", wanderAIAgentInitParams[0]);
            AIController.instance.CreateAI(8, 0, "AIBorn0", wanderAIAgentInitParams[1]);
            AIController.instance.CreateAI(5, 1, "AIBorn1", turrentAIAgentInitParams[0]);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
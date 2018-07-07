using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OperationTrident.Common.AI
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField]
        WanderAIAgentInitParams[] wanderAIAgentInitParams;

        // Use this for initialization
        void Start()
        {
            AIController.instance.CreateAI(wanderAIAgentInitParams.Length, 0, "SwopPoints", wanderAIAgentInitParams);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
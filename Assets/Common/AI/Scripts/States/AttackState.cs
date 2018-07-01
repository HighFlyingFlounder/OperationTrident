using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    [DisallowMultipleComponent]
    public class AttackState : AIState
    {
        public static readonly string STATE_NAME = "Attack";

        public class Conditions
        {
            public static readonly string LOSE_TARGET = "Lose Target";
        }

        [System.Serializable]
        public class InitParams : InitParamsBase
        {
            public Vector3 targetPosition;
        }

        public override void Init(AIStateParam param)
        {
            base.Init(param);
            _paramParser = new AttackStateParamParser(param);
        }
        public override string Execute()
        {
            Debug.Log("Attack player in " + (_paramParser as AttackStateParamParser).TargetPosition);

            // Ray ray = new Ray(transform.position, transform.forward);
            // RaycastHit hit;

            // if (Physics.SphereCast(ray, 0.75f, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Map") | 1 << LayerMask.NameToLayer("Players")))
            // {
            //     GameObject hitObj = hit.transform.gameObject;
            //     if (hitObj.GetComponent<PlayerCharacter>() == null)
            //     {
            //         _satisfy = Conditions.LOSE_TARGET;
            //     }
            // }

            return _satisfy;
        }
    }

    public class AttackStateParamParser : AIStateParamParserBase
    {
        public AttackStateParamParser(AIStateParam param) : base(param) { }

        public Vector3 TargetPosition
        {
            get
            {
                object position = _param.GetMassData("targetPosition");
                if (position != null)
                {
                    return (Vector3)position;
                }
                else
                {
                    Debug.Log("Can not get 'targetPosition' in 'AttackStateParamParser'.");
                    throw new System.NotImplementedException();
                }
            }
        }
    }
}

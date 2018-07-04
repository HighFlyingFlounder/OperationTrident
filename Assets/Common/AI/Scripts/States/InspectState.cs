using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    [DisallowMultipleComponent]
    public class InspectState : AIState
    {
        public static readonly string STATE_NAME = "Inspect";

        public class Conditions
        {
            public static readonly string SIGHT_PLAYER = "Sight Player";
            public static readonly string FINISH_INSPECTION = "Finish Inspection";
        }

        Animator _animator;
        float _duration;

        public override void Init()
        {
            if (IsFirstInit)
            {
                _animator = GetComponent<Animator>();
            }
            _animator.SetBool("FindTarget", true);
            _duration = 8f;
        }

        public override string Execute()
        {
            Transform target = Utility.DetectPlayers(_agent.Camera);
            if (target != null)
            {
                _agent.Target = target;
                return Conditions.SIGHT_PLAYER;
            }

            _duration -= Time.deltaTime;
            if (_duration < 0)
            {
                return Conditions.FINISH_INSPECTION;
            }

            return null;
        }

        public override void Exit()
        {
            _animator.SetBool("FindTarget", false);
        }
    }
}

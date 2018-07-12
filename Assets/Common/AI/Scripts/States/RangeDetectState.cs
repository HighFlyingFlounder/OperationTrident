using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    [DisallowMultipleComponent]
    public class RangeDetectState : AIState
    {
        public static readonly string STATE_NAME = "Range Detect";

        public class Conditions
        {
            public static readonly string SIGHT_PLAYER = "Sight Player";
        }

        float _depressionAngle;
        float _rangeMax;
        public override void Init()
        {
            if (IsFirstInit)
            {
                _depressionAngle = _agent.DepressionAngle;
                _rangeMax = _agent.DetectRangeMax;
            }
        }

        public override string Execute()
        {
            _agent.Target = Utility.RangeDetectAll(_agent.Center.position, _depressionAngle);
            if(_agent.Target != null)
            {
                return Conditions.SIGHT_PLAYER;
            }
            return null;
        }

        public override void Exit()
        {
        }

    }
}

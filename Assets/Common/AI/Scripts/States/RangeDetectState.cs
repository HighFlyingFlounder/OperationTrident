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

        float _rangeMin;
        float _rangeMax;
        public override void Init()
        {
            if (IsFirstInit)
            {
                _rangeMin = _agent.DetectRangeMin;
                _rangeMax = _agent.DetectRangeMax;
            }
        }

        public override string Execute()
        {
            Transform[] players = Utility.GetPlayersPosition();
			Vector3 origin = transform.position;

            foreach (var player in players)
            {
                Vector3 direction = player.position - origin;
                direction.y = 0;
                if (direction.magnitude > _rangeMax || direction.magnitude < _rangeMin)
                    continue;

                Ray ray = new Ray(origin, player.position - origin);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~(LayerMask.GetMask("IgnoreBullets") | LayerMask.GetMask("Enemy"))))
                {
                    if (hit.collider.tag == "Player")
                    {
                        _agent.Target = player;
                        return Conditions.SIGHT_PLAYER;
                    }
                }
            }

            return null;
        }

        public override void Exit()
        {
        }

    }
}

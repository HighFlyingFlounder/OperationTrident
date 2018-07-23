using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    [DisallowMultipleComponent]
    public class TrackRandomPlayerState : AIState
    {
        public static readonly string STATE_NAME = "Track Random Player";

        public class Conditions
        {
            public static readonly string FINISH_TRACK = "Finish Track";
        }

        public override void Init()
        {
        }

        public override string Execute()
        {
            Transform[] players = Utility.GetPlayersPosition();
            if (players.Length != 0)
                _agent.Target = players[Random.Range(0, players.Length)];
            return Conditions.FINISH_TRACK;
        }

        public override void Exit()
        {
        }
    }
}
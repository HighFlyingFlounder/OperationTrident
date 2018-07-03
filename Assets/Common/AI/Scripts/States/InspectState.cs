﻿using System.Collections;
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

        AICamera _camera = null;
        Animator _animator;
        float _duration;

        public override void Init()
        {
            base.Init();
            InitOnce();
            _animator.SetBool("FindTarget", true);
            _duration = 8f;
        }

        public override void InitOnce()
        {
            if (_firstInit)
            {
                base.InitOnce();
                _animator = GetComponent<Animator>();
                _camera = GetComponent<AIAgent>().Camera;
            }
        }

        public override string Execute()
        {
            _duration -= Time.deltaTime;
            if (_duration < 0)
            {
                _satisfy = Conditions.FINISH_INSPECTION;
            }

            Transform target = Utility.DetectPlayers(_camera);
            if(target != null)
            {
                GetComponent<AIAgent>().Target = target;
                _satisfy = Conditions.SIGHT_PLAYER;
                return _satisfy;
            }

            return _satisfy;
        }

        public override void Exit() 
        {
            _animator.SetBool("FindTarget", false);
        }
    }
}

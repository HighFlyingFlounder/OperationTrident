using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace OperationTrident.Common.AI
{
    public class AlertState : AIState
    {
        public static readonly string STATE_NAME = "Alert";

        public class Conditions
        {
            public static readonly string SIGHT_PLAYER = "Sight Player";
            public static readonly string LOST_PLAYER = "Lost Player";
        }
        AICamera _camera = null;
        Animator _animator;

        public override void Init()
        {
            base.Init();
            InitOnce();
            _animator.SetBool("TargetDetected", true);
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
            transform.LookAt(GetComponent<AIAgent>().Target);
            _camera.UpdateCamera();

            if (_camera.DetectTarget(GetComponent<AIAgent>().Target))
            {
                // Debug.Log(Conditions.SIGHT_PLAYER);
                // _satisfy = Conditions.SIGHT_PLAYER;
            }
            else
            {
                Debug.Log(Conditions.LOST_PLAYER);
                _satisfy = Conditions.LOST_PLAYER;
            }

            return _satisfy;
        }

        public override void Exit()
        {
            _animator.SetBool("TargetDetected", false);
        }
    }
}
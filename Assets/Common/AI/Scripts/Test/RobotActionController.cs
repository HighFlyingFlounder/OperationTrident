using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class RobotActionController : AIActionController
    {
        [SerializeField]
        Animator _animator;

        [SerializeField]
        RobotShooter _shooter;

        public override void Move(bool isStart)
        {
            _animator.SetBool("Move", isStart);
        }

        public override void FindTarget(bool isStart)
        {
            _animator.SetBool("FindTarget", isStart);
        }

        public override void DetectedTarget(bool isStart)
        {
            _animator.SetBool("TargetDetected", isStart);
        }

        public override void Shoot(Vector3 shootingPoint)
        {
            _shooter.Shoot(shootingPoint);
        }

        public override void LookAt(Vector3 interestPoint)
        {
            transform.forward = Utility.GetDirectionOnXOZ(transform.position, interestPoint);
        }
    }
}

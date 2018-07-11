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

        public override void Shoot(string shootingTargetName)
        {
            _shooter.Shoot(Utility.GetPlayerByName(shootingTargetName).position);
        }

        // public override void LookAt(Vector3 interestPoint)
        // {
        //     transform.forward = Utility.GetDirectionOnXOZ(transform.position, interestPoint).normalized;
        // }

        public override IEnumerator Destroy()
        {
            int duration = 15;
            for (int i = 0; i < duration; i++)
            {
                transform.Rotate(-90 / duration, 0, 0);
                yield return null;
            }
            yield return new WaitForSeconds(.2f);
            Destroy(gameObject);
        }

        public override bool DetectPlayer(Transform player)
        {
            throw new System.NotImplementedException();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class TurretActionController : AIActionController
    {
        [SerializeField]
        TurretShooter _shooter;

        public override void LookAt(Vector3 interestPoint)
        {
            Transform joint = transform.Find("Rotation Joint");
            joint.right = -(interestPoint - joint.position).normalized;
        }

        public override void Shoot(Vector3 shootingPoint)
        {
            _shooter.Shoot(shootingPoint);
        }

        public override IEnumerator Destroy()
        {
			yield return new WaitForSeconds(.2f);
			Destroy(gameObject);
        }
    }
}
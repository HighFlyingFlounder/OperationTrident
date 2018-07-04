using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class AIActionController : MonoBehaviour
    {
        public virtual void Move(bool isStart) { }

        public virtual void FindTarget(bool isStart) { }

        public virtual void DetectedTarget(bool isStart) { }

        public virtual void Shoot(Vector3 shootingPoint) { }

        public virtual void LookAt(Vector3 interestPoint) { }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Forge3D;

namespace OperationTrident.Common.AI
{
    public class TurretActionController : AIActionController
    {
        [SerializeField]
        AIWeapon _weapon;
        [SerializeField]
        F3DTurret _turret;

        Transform _target = null;
        bool isShooting = false;

        public override void LookAt(Transform target)
        {
            _target = target;
            // Transform joint = transform.Find("Rotation Joint");
            // joint.right = -(interestPoint - joint.position).normalized;
        }

        public override void StopLookAt()
        {
            _target = null;
        }

        public override void Shoot(Transform shootingTarget)
        {
            // _target = transform.GetComponent<AIAgent>().Target;
            // // _shooter.Shoot(shootingPoint);
            // LookAt(shootingTarget.position);
            _weapon.Shoot();
            // isShooting = true;
            // shootTime += transform.GetComponent<AIAgent>().fsmUpdateTime;
        }

        public override void StopShoot(){
            _weapon.StopShoot();
        }

        public override IEnumerator Destroy()
        {
			yield return new WaitForSeconds(.2f);
			Destroy(gameObject);
        }

        private void Update()
        {
            if(_target != null)
                _turret.SetNewTarget(_target.position);
            // if(_target != null)
            // {
            //     LookAt(_target.position);
            //     _weapon.Shoot();
            //     isShooting = true;
            // }
            // else if(isShooting)
            // {
            //     _weapon.StopShoot();
            // }
            // shootTime -= Time.deltaTime;
            // if(shootTime < 0)
            // {
            //     shootTime = 0;
            //     if(isShooting)
            //     {
            //         isShooting = false;
            //         _weapon.StopShoot();
            //     }
            // }
        }

        public override bool DetectPlayer(Transform player)
        {
            AIAgent agent = transform.GetComponent<AIAgent>();
            return Utility.RangeDetect(player, agent.Center.position, agent.DepressionAngle);
        }
    }
}
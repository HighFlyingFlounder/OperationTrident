using System.Collections;
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

        public override void LookAtWithTargetName(string shootingTargetName)
        {
            _target = Utility.GetPlayerByName(shootingTargetName);
        }

        public override void StopLookAt()
        {
            _target = null;
        }

        public override void Shoot()
        {
            _weapon.Shoot();
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
        }

        public override bool DetectPlayer(Transform player)
        {
            AIAgent agent = transform.GetComponent<AIAgent>();
            if (player == null)
                return false;
            return Utility.RangeDetect(player, agent.Center.position, agent.DepressionAngle);
        }
    }
}
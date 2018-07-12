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
        AIWeapon _weapon;

        [SerializeField]
        float _rotateSpeed = 5;

        [SerializeField]
        Transform _arms = null;

        Transform _target = null;

        public override void FindTarget(bool isStart)
        {
            _animator.SetBool("FindTarget", isStart);
        }

        public override void Shoot()
        {
            // _shooter.Shoot(Utility.GetPlayerByName(shootingTargetName).position);
            _weapon.Shoot();
        }
        public override void StopShoot() 
        {
            _weapon.StopShoot();
        }

        // public override void LookAt(Vector3 interestPoint)
        // {
        //     transform.forward = Utility.GetDirectionOnXOZ(transform.position, interestPoint).normalized;
        // }
        public override void LookAt(string targetName)
        {
            _target = Utility.GetPlayerByName(targetName);
        }

        public override void StopLookAt()
        {
            _target = null;
        }

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

        private void Update()
        {
            if (_target != null)
            {
                Vector3 _forward = Utility.GetDirectionOnXOZ(transform.position, _target.position);

                Quaternion targetRotationY = Quaternion.LookRotation(_forward, transform.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationY, _rotateSpeed * Time.deltaTime);
                transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
                
                Vector3 _direction = _target.position - _arms.transform.position;
                Quaternion targetRotationX = Quaternion.LookRotation(_direction, _arms.transform.up);
                _arms.transform.rotation = Quaternion.Slerp(_arms.transform.rotation, targetRotationX, _rotateSpeed * Time.deltaTime);
                _arms.transform.localEulerAngles = new Vector3(_arms.transform.localEulerAngles.x, 0f, 0f);

            }
            else
            {
                Quaternion targetRotationX = Quaternion.LookRotation(transform.forward, _arms.transform.up);
                _arms.transform.rotation = Quaternion.Slerp(_arms.transform.rotation, targetRotationX, _rotateSpeed * Time.deltaTime);
                _arms.transform.localEulerAngles = new Vector3(_arms.transform.localEulerAngles.x, 0f, 0f);
            }
        }
    }
}

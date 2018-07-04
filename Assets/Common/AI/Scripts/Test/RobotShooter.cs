using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotShooter : MonoBehaviour
{
    [SerializeField]
    Transform[] _weaponList;
    bool _isShoot = false;
    Vector3 _target;

    void LateUpdate()
    {
		if(!_isShoot)
			return;

		_isShoot = false;
        foreach (var weapon in _weaponList)
        {
            weapon.LookAt(_target);
            Vector3 origin = weapon.Find("Gun Point").position;
            Debug.DrawLine(origin, _target, Color.red);
        }
    }
    public void Shoot(Vector3 target)
    {
		_target = target;
		_isShoot = true;
    }
}

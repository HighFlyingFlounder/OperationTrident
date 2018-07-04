using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotShooter : MonoBehaviour
{
    [SerializeField]
    Transform[] _weaponList;

    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    int bulletQuantity;

    [SerializeField]
    float fireRate;

    bool _isShoot = false;
    Vector3 _target;

    float _time = 0;
    int _weaponIndex = 0;

    void LateUpdate()
    {
		if(!_isShoot)
			return;

		_isShoot = false;
       
        _weaponList[_weaponIndex].LookAt(_target);
        Transform gunPoint = _weaponList[_weaponIndex].Find("Gun Point");
        Vector3 origin = gunPoint.position;

        if(_time < 0)
        {
            _time = fireRate;
            Debug.DrawLine(origin, _target, Color.red);
            GameObject bullet = Instantiate(bulletPrefab) as GameObject;
            bullet.transform.position = gunPoint.position;
            bullet.transform.up = (_target - origin).normalized;
            _weaponIndex = (_weaponIndex + 1) % _weaponList.Length;
        }
        
    }
    public void Shoot(Vector3 target)
    {
        _time -= Time.deltaTime;
		_target = target;
		_isShoot = true;
    }
}

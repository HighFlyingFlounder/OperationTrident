using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShooter : MonoBehaviour {

	[SerializeField]
    Transform cannon;

    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    int bulletQuantity;

    [SerializeField]
    float fireRate;

	float _time = 0;

	public void Shoot(Vector3 target)
    {
		Transform gunPoint = cannon.Find("Gun Point");
        Debug.DrawLine(gunPoint.position, target, Color.red);
		_time -= Time.deltaTime;
		if(_time < 0)
		{
			_time = fireRate;
			GameObject bullet = Instantiate(bulletPrefab) as GameObject;
            bullet.transform.position = gunPoint.position;
            bullet.transform.up = (target - gunPoint.position).normalized;
		}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room1
{
    public class BulletGenerator
    {

        public static void GeneratorBullet(Ray ray,GameObject bulletPrefab, string attacker, bool fromAI, float speed=500.0f,float gravity=-9.8f)
        {
            GameObject bullet = Object.Instantiate(bulletPrefab) as GameObject;
            bullet.GetComponent<BulletScript>().StartWithRay(ray, attacker,fromAI,speed, gravity);
        }

    }
}

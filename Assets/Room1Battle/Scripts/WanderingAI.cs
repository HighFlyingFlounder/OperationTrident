using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room1
{
    public class WanderingAI : MonoBehaviour
    {
        public float speed = 3.0f;
        public const float baseSpeed = 3.0f;
        public float obstacleRange = 5.0f;
        public float gravity = -9.8f;
        private bool onFloor;
        private bool alive;
        private bool foundPlayer;
        private double timeCount;

        private GameObject player;

        [SerializeField]
        private GameObject bulletPrefab;
        private GameObject bullet;

        public void SetAlive(bool _alive)
        {
            alive = _alive;
        }

        // Use this for initialization
        void Start()
        {
            alive = true;
            foundPlayer = false;
            onFloor = false;
            timeCount = 0;
        }

        private void OnSpeedChanged(float value)
        {
            speed = baseSpeed * value;
            //Debug.Log("value: " + value);
            //Debug.Log("speed: " + speed);
        }

        private void OnCollisionEnter(Collision collision)
        {
            //Debug.Log(collision.collider.gameObject.name);
            if (collision.collider.gameObject.name == "Floor")
                onFloor = true;
        }

        // Update is called once per frame
        void Update()
        {
            timeCount += Time.deltaTime;
            if (alive)
            {
                //如果不在地板（在空中）就掉落
                if (!onFloor)
                    transform.Translate(0, gravity, speed * Time.deltaTime);
                else
                    transform.Translate(0, 0, speed * Time.deltaTime);
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;

                if (Physics.SphereCast(ray, 0.75f, out hit))
                {
                    GameObject hitObject = hit.transform.gameObject;
                    if (hitObject.CompareTag("Player"))//检查目标对象是否是玩家(玩家在面前)
                    {
                        foundPlayer = true;
                        player = hitObject;
                        if (bullet == null)
                        {
                            //bullet = Instantiate(bulletPrefab) as GameObject;
                            BulletGenerator.GeneratorBullet(ray, bulletPrefab, gameObject.name, true);
                        }

                    }
                    if (foundPlayer)
                    {
                        //发现玩家后，每次都会逼近玩家
                        transform.forward = player.transform.position - transform.position;
                    }
                    else if ((int)timeCount % 4 == 0)
                    {
                        float angle = UnityEngine.Random.Range(-30, 30);
                        transform.Rotate(0, angle, 0);
                        timeCount += 1;
                    }
                    else
                    {
                        if (hit.distance < obstacleRange
                            && hitObject.GetComponent<BulletScript>() == null
                            && !hitObject.CompareTag("Player"))
                        {
                            float angle = UnityEngine.Random.Range(-110, 110);
                            transform.Rotate(0, angle, 0);
                        }
                    }
                }

            }
        }
    }
}
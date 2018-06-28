using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room1 {
    public class BulletScript : MonoBehaviour {

        public float speed = 500.0f;

        public float gravity = -9.8f;

        public Vector3 direction;

        // Use this for initialization
        void Start() {

        }

        public void StartWithRay(Ray initRay,float speed=500.0f,float gravity=-9.8f)
        {
            this.speed = speed;
            this.gravity = gravity;
            direction = initRay.direction;
            transform.position = initRay.origin;
        }

        // Update is called once per frame
        void Update() {


            transform.localPosition =
                new Vector3(
                    transform.localPosition.x + speed * direction.x * Time.deltaTime,
                    transform.localPosition.y + speed * direction.y * Time.deltaTime,
                    transform.localPosition.z + speed * direction.z * Time.deltaTime);
            transform.position =
                new Vector3
                (transform.position.x,
                transform.position.y + gravity * Time.deltaTime,
                transform.position.z);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<ReactiveTarget>() != null)
            {
               other.GetComponent<ReactiveTarget>().ReactToHit();
                Debug.Log("打中了敌人");
            }
            Destroy(this.gameObject);
        }

        public void OnCollisionEnter(Collision collision)
        {

            Debug.Log("1242156146");
            if (collision.gameObject.GetComponent<ReactiveTarget>() != null)
            {
                collision.gameObject.GetComponent<ReactiveTarget>().ReactToHit();
                Debug.Log("打中了敌人");
            }
            Destroy(this.gameObject);
        }
    }
}

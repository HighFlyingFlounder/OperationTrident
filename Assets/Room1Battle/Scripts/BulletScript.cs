using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room1 {
    public class BulletScript : MonoBehaviour {

        public float speed = 500.0f;

        public float gravity = -9.8f;

        public Vector3 direction;

        private float timer;

        // Use this for initialization
        void Start() {
            timer = 0.0f;
        }

        public void StartWithRay(Ray initRay,float speed=500.0f,float gravity=-9.8f)
        {
            this.speed = speed;
            this.gravity = gravity;
            direction = initRay.direction;
            transform.position = initRay.origin + new Vector3(direction.x, direction.y, direction.z);
        }

        // Update is called once per frame
        void Update() {
            try
            {
                timer += Time.deltaTime;
                if (timer > 10.0f) Destroy(gameObject);
                Vector3 originPoint = transform.position;


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

                Vector3 afterPoint = transform.position;
                Vector3 directionFrame = afterPoint - originPoint;
                Ray ray = new Ray(originPoint, directionFrame);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject hitObject = hit.transform.gameObject;

                    if (Vector3.Distance(originPoint, hitObject.transform.position) <= speed || hitObject.GetComponent<ReactiveTarget>() != null)
                    {
                        //hitObject.GetComponent<ReactiveTarget>().OnHit();
                        Debug.Log("打中了敌人");
                    }
                    Destroy(gameObject);
                    Destroy(this);
                    return;
                }
            }
            catch(System.Exception e)
            {
                //other.GetComponent<ReactiveTarget>().OnHit("1",1);
                Debug.Log("打中了敌人");
            }
        }
        public void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<BulletScript>() != null) return;
            if (other.CompareTag("Player")) return;
            if (other.GetComponent<ReactiveTarget>() != null)
            {
                //collision.gameObject.GetComponent<ReactiveTarget>().OnHit("1",1);
                Debug.Log("打中了敌人");
            }
            Destroy(this.gameObject);
        }

    }
}

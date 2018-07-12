using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room1 {
    public class BulletScript : MonoBehaviour {

        public float speed = 500.0f;

        public float gravity = -9.8f;

        public Vector3 direction;

        private float timer;

        private string attacker;

        private bool fromAI;

        // Use this for initialization
        void Start() {
            timer = 0.0f;
        }

        public void StartWithRay(Ray initRay,string _attacker,bool _fromAI,float speed=500.0f,float gravity=-9.8f)
        {
            this.speed = speed;
            this.gravity = gravity;
            direction = initRay.direction;
            transform.position = initRay.origin + new Vector3(direction.x, direction.y, direction.z);
            attacker = _attacker;
            fromAI = _fromAI;
        }

        // Update is called once per frame
        void Update() {
            if (direction == new Vector3(0.0f, 0.0f, 0.0f) || direction == null) return;
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

                    if (Vector3.Distance(originPoint, hitObject.transform.position) <= speed || hitObject.GetComponent<Common.ReactiveTarget>() != null)
                    {
                        hitObject.GetComponent<Common.ReactiveTarget>().OnHit(attacker, fromAI, 1);
                        Debug.Log("打中了敌人");
                    }
                    Destroy(gameObject);
                    Destroy(this);
                    return;
                }
            }
            catch(System.Exception e)
            {
            }
        }
        public void OnTriggerEnter(Collider other)
        {
            Debug.LogFormat("Reach OnTriggerEnter， other.gameObject.name = {0}", other.gameObject.name);
            if (other.GetComponent<BulletScript>() != null) return;
            // 玩家不会被击中
            //if (other.CompareTag("Player")) return;
            if (other.gameObject.GetComponent<Common.ReactiveTarget>() != null)
            {
                other.gameObject.GetComponent<Common.ReactiveTarget>().OnHit(attacker,fromAI,1);
                Debug.Log("打中了敌人");
            }
            Destroy(gameObject);
        }

    }
}

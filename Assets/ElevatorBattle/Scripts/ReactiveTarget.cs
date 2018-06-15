using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Elevator
{
    public class ReactiveTarget : MonoBehaviour
    {
        // 是否死亡
        private bool dead;

        public bool Dead
        {
            get
            {
                return dead;
            }
        }

        // Use this for initialization
        void Start()
        {
            dead = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ReactToHit()
        {
            if (!dead)
            {
                dead = true;
                StartCoroutine(Die());
            }
        }

        private IEnumerator Die()
        {
            this.transform.Rotate(-75, 0, 0);
            yield return new WaitForSeconds(0.2f);
            gameObject.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            Destroy(this.gameObject);
        }
    }
}
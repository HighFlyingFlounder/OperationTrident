using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room1
{
    public class ReactiveTarget : MonoBehaviour
    {
        // 是否死亡
        private bool dead;
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
            yield return new WaitForSeconds(1.5f);
            Destroy(this.gameObject);
        }
    }
}
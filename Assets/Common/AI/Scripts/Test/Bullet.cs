using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class Bullet : MonoBehaviour
    {
		[SerializeField]
		float speed;
        // Update is called once per frame
        void Update()
        {
			transform.Translate(0, speed * Time.deltaTime, 0);
        }
		private void OnCollisionEnter(Collision other)
		{
			Debug.Log(other.gameObject.tag);
			Destroy(gameObject);
		}
		
    }
}

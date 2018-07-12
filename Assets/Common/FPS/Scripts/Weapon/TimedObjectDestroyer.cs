using UnityEngine;
using System.Collections;

namespace OperationTrident.FPS.Weapons {

    public class TimedObjectDestroyer : MonoBehaviour {
        public float LifeTime = 10.0f;

        // Use this for initialization
        void Start() {
            Destroy(gameObject, LifeTime);
        }
    }
}

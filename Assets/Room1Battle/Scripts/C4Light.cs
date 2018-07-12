using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room1
{
    public class C4Light : MonoBehaviour
    {
        [SerializeField]
        private LensFlare les;

        public float changeSpeed=0.05f;
        public float maxBrightness = 0.6f;
        public float minBrightness = 0.1f;
        private bool enlight = true;
        // Use this for initialization
        void Start()
        {
            les.brightness = minBrightness;
        }

        // Update is called once per frame
        void Update()
        {
            if (enlight)
            {
                les.brightness += changeSpeed;
                if (les.brightness >= maxBrightness) enlight = false;
            }
            else
            {
                les.brightness -= changeSpeed;
                if (les.brightness <= minBrightness) enlight = true;
            }
        }
    }
}

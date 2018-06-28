using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OperationTrident.Weapons {
    [System.Serializable]
    public class BulletHolePool : MonoBehaviour {
        //保存所有弹孔Object
        public List<GameObject> BulletHoles = new List<GameObject>();
        public GameObject replacementBulletHole;                                // A replacement bullet hole prefab to be instantiated when a bullet hole has been destroyed

        //用于确定下一个使用的弹孔Object
        private int m_CurrentIndex = 0;

        // Use this for initialization
        void Start() {
            if (replacementBulletHole == null) {
                Debug.LogWarning("The Replacement Bullet Hole for " + gameObject.name + " is null.  Please set this variable in the inspector.");
                replacementBulletHole = new GameObject();
            }
        }


        // Increment the current index - a method is used for this so that every time it's incremented, we also check and make sure the index hasn't yet reached number of bullet holes in the pool
        private void IncrementIndex() {
            // Add 1 to the index - because this one really needed to have a comment...
            m_CurrentIndex++;

            // If the index reaches the number of elements in the list, we want to cycle back to the beginning
            if (m_CurrentIndex >= BulletHoles.Count)
                m_CurrentIndex = 0;
        }

        // Place the next bullet hole at the specified position and rotation
        public void PlaceBulletHole(Vector3 pos, Quaternion rot) {
            // Make sure the current bullet hole still exists
            VerifyBulletHole();

            // Now the bullet hole is ready to be re-positioned

            // Start by clearing the parent.  This prevents problems with the transform inherited from previous parents when the bullet hole GameObject is re-parented
            BulletHoles[m_CurrentIndex].transform.parent = null;

            // Now set the position and rotation of the bullet hole
            BulletHoles[m_CurrentIndex].transform.position = pos;
            BulletHoles[m_CurrentIndex].transform.rotation = rot;
            BulletHoles[m_CurrentIndex].transform.localScale = BulletHoles[m_CurrentIndex].transform.localScale;

            // Now refresh the bullet hole so it can be re-parented, etc.
            if (BulletHoles[m_CurrentIndex].GetComponent<BulletHole>() == null)
                BulletHoles[m_CurrentIndex].AddComponent<BulletHole>();
            BulletHoles[m_CurrentIndex].GetComponent<BulletHole>().Refresh();

            // Now increment our index so the oldest bullet holes will always be the first to be re-used
            IncrementIndex();
        }

        // Verify that the specified bullet hole still exists
        private void VerifyBulletHole() {
            // If the bullet hole at the current index has been destroyed, instantiate a new one
            if (BulletHoles[m_CurrentIndex] == null) {
                GameObject bh = Instantiate(replacementBulletHole, transform.position, transform.rotation) as GameObject;
                BulletHoles[m_CurrentIndex] = bh;
            }
        }

    }


}
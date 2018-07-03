using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Elevator
{
    public class Wall : MonoBehaviour
    {
        private GameObject child1;
        private GameObject child2;

        [SerializeField]
        public float speed = 1.0f;

        // Use this for initialization
        void Start()
        {
            child1 = GameObject.Find("Mesh1");
            child2 = GameObject.Find("Mesh2");
        }

        // Update is called once per frame
        void Update()
        {
            switch (SceneController.state)
            {
                case SceneController.ElevatorState.Ready:
                    break;
                case SceneController.ElevatorState.Start_Fighting:
                    break;
                case SceneController.ElevatorState.Fighting:
                    //若到一定高度则替换掉
                    if (child1.transform.position.y >= 90)
                    {
                        child1.transform.position -= new Vector3(0, child1.transform.position.y - child2.transform.position.y + 100, 0);
                    }

                    if (child2.transform.position.y >= 90)
                    {
                        child2.transform.position -= new Vector3(0, child2.transform.position.y - child1.transform.position.y + 100, 0);
                    }

                    //移动
                    child1.transform.position += new Vector3(0, speed, 0);
                    child2.transform.position += new Vector3(0, speed, 0);
                    break;
                case SceneController.ElevatorState.End:
                    if (child1.transform.position.y < 0 || child2.transform.position.y < 0)
                    {
                        //移动
                        child1.transform.position += new Vector3(0, speed, 0);
                        child2.transform.position += new Vector3(0, speed, 0);

                        if(child1.transform.position.y >= 0 && child2.transform.position.y >= 0)
                        {
                            child1.transform.position -= new Vector3(0, child1.transform.position.y, 0);
                            child2.transform.position -= new Vector3(0, child2.transform.position.y, 0);
                        }
                    }
                    break;
            }
        }
    }
}

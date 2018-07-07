using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Elevator
{
    public class Wall : MonoBehaviour
    {
        private GameObject child1;
        private GameObject child2;
        private GameObject child3;

        private GameObject door;
        private GameObject d1;
        private GameObject d2;
        private GameObject d3;

        private float height;
        private float pos;
        private float origin;

        private float dis;

        private bool stop = false;

        public static bool state = false;

        [SerializeField]
        public float speed = 1.0f;

        // Use this for initialization
        void Start()
        {
            child1 = GameObject.Find("wall1");
            child2 = GameObject.Find("wall2");
            child3 = GameObject.Find("wall3");

            door = GameObject.Find("Door");
            d1 = GameObject.Find("d1");
            d2 = GameObject.Find("d2");
            d3 = GameObject.Find("d3");

            height = child1.transform.position.y - child2.transform.position.y;
            pos = child1.transform.position.y + 1.5f * height;
            origin = child1.transform.position.y;
        }

        // Update is called once per frame
        void Update()
        {
            switch (SceneController.state)
            {
                case SceneController.ElevatorState.Initing:
                    d1.SetActive(false);
                    break;

                case SceneController.ElevatorState.Ready:
                    break;

                case SceneController.ElevatorState.Start_Fighting:
                    door.SetActive(false);
                    d1.SetActive(true);
                    break;

                case SceneController.ElevatorState.Fighting:
                    //若到一定高度则替换掉
                    if (child1.transform.position.y >= pos)
                    {
                        child1.transform.position -= new Vector3(0, 3 * height, 0);
                    }

                    if (child2.transform.position.y >= pos)
                    {
                        child2.transform.position -= new Vector3(0, 3 * height, 0);
                    }

                    if (child3.transform.position.y >= pos)
                    {
                        child3.transform.position -= new Vector3(0, 3 * height, 0);
                    }

                    //移动
                    child1.transform.position += new Vector3(0, speed, 0);
                    child2.transform.position += new Vector3(0, speed, 0);
                    child3.transform.position += new Vector3(0, speed, 0);
                    break;

                case SceneController.ElevatorState.End:
                    if (!stop)
                    {
                        List<float> d = new List<float>();

                        d.Add(child1.transform.position.y - origin);
                        d.Add(child2.transform.position.y - origin);
                        d.Add(child3.transform.position.y - origin);
                        d.Sort();

                        if (d[1] < 0)
                            dis = d[1];
                        else
                            dis = d[0];

                        stop = true;
                    }


                    if (dis < 0)
                    {
                        //若到一定高度则替换掉
                        if (child1.transform.position.y >= pos)
                        {
                            child1.transform.position -= new Vector3(0, 3 * height, 0);
                        }

                        if (child2.transform.position.y >= pos)
                        {
                            child2.transform.position -= new Vector3(0, 3 * height, 0);
                        }

                        if (child3.transform.position.y >= pos)
                        {
                            child3.transform.position -= new Vector3(0, 3 * height, 0);
                        }

                        //移动
                        child1.transform.position += new Vector3(0, speed, 0);
                        child2.transform.position += new Vector3(0, speed, 0);
                        child3.transform.position += new Vector3(0, speed, 0);

                        dis += speed;

                        if (dis >= 0)
                        {
                            child1.transform.position -= new Vector3(0, dis, 0);
                            child2.transform.position -= new Vector3(0, dis, 0);
                            child3.transform.position -= new Vector3(0, dis, 0);

                            state = true;

                            door.SetActive(true);
                            if (child1.transform.position.y == origin)
                                d1.SetActive(false);
                            if (child2.transform.position.y == origin)
                                d2.SetActive(false);
                            if (child3.transform.position.y == origin)
                                d3.SetActive(false);
                        }
                    }

                    break;
            }
        }
    }
}

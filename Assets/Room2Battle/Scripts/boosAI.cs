using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle
{
    public class boosAI : MonoBehaviour
    {
        [SerializeField]
        protected GameObject[] Missiles;

        [SerializeField]
        protected missilLauncher[] pos;

        [SerializeField]
        protected Transform target;

        [SerializeField]
        protected float radius = 5.0f;

        //导弹齐射完毕与否
        protected bool isShotDone = true;

        void Update()
        {

            foreach (missilLauncher rocket in pos)
            {
                float r = 3.0f;
                Transform temp = target;
                //temp.position = new Vector3(target.position.x + Random.Range(-r, r), target.position.y , target.position.z + Random.Range(-r, r));

                rocket.SetTargetPostion(temp);
                rocket.launch();
            }


        }
        /*
        /// <summary>
        /// 使用携程隔一段时间发一枚导弹
        /// </summary>
        /// <returns></returns>
        IEnumerator shotTogether()
        {
            isShotDone = false;
            float r = radius;
            for (int j = 0; j < 3; ++j)
            {
                r += 3;
                for (int i = 0; i < pos.Length; ++i)
                {
                    Vector3 pos_ = target.position;
                    pos_.x += Random.Range(-r, r);
                    pos_.y += Random.Range(-r, r);

                    //Quaternion targetRotation = Quaternion.LookRotation(target.position - pos[i].position);


                    //pos[i].rotation = targetRotation;
                    Transform p = pos[i];   
                    p.LookAt(pos_);

                    Vector3 vec = new Vector3(p.eulerAngles.x, p.eulerAngles.y, p.eulerAngles.z);
                    Quaternion ro = p.rotation;

                    p.eulerAngles = new Vector3(-90,p.eulerAngles.y,0);

                    Transform t = pos[i].Find("Rocket launcher");

                    t.LookAt(pos_, t.up);
                    t.localEulerAngles = new Vector3(t.localEulerAngles.x+90, t.localEulerAngles.y, t.localEulerAngles.z);



                    Instantiate(Missiles[Random.Range(0, Missiles.Length)], p.position,ro);


                    yield return new WaitForSeconds(Random.Range(1.0f,2.0f));
                }
            }
            isShotDone = true;
        }
        */
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boosAI : MonoBehaviour {
    [SerializeField]
    protected GameObject[] Missiles;

    [SerializeField]
    protected Transform[] pos;

    [SerializeField]
    protected Transform target;

    [SerializeField]
    protected float radius = 5.0f;

    //导弹齐射完毕与否
    protected bool isShotDone = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (isShotDone)
            {
                StartCoroutine(shotTogether());
            }
        }

    }
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
                Quaternion ro = p.rotation;

                p.eulerAngles = new Vector3(-90,p.eulerAngles.y,0);
                Instantiate(Missiles[Random.Range(0, Missiles.Length)], p.position,ro);
              

                yield return new WaitForSeconds(Random.Range(1.0f,2.0f));
            }
        }
        isShotDone = true;
    }

}

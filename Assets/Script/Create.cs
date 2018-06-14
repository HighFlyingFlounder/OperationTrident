using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : MonoBehaviour {
    [SerializeField]
    //要生成的物体
    public GameObject CreateObject;
    //要生成的数量
    public int number;
    //生成速度
    public float time;
    //是否开始生成物体
    public bool isStart;

    //协程
    private IEnumerator coroutine;
    //维护一个物体列表
    public List<GameObject> list;

    // Use this for initialization
    void Start () {
        //一秒执行一次
        coroutine = WaitAndPrint(time);

        number = 10;
        time = 1;
        isStart = false;
        list = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
        //判断何时生成
        if (isStart)
        {
            //开始生成
            StartCoroutine(coroutine);
            isStart = false;
        }

        if (list.Count % number == 0)
        {
            //停止生成器
            StopCoroutine(coroutine);
            //留种
            coroutine = WaitAndPrint(time);
        }
    }

    //协程coroutine的存在，配合下面的yield return回调，这段代码将被1s执行一次
    private IEnumerator WaitAndPrint(float waitTime)
    {
        for (int i = 0; i < number;)
        {
            //生成物体
            GameObject o = Instantiate(CreateObject, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
            o.name = list.Count.ToString();
            list.Add(o);
            i++;
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void Operate()
    {
        isStart = true;
    }
}

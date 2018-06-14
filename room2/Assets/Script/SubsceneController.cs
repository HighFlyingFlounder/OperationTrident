using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//===============================================================================================
//=========================        大的状态机，负责管理子场景的切换 ===============================
//===============================================================================================


/// <summary>
/// 这是一个状态机类，主要负责游戏状态的状态边的转移，状态的注册
/// 1：状态边并没有储存在SubStateController中，而是储存在每一个SubState中，
/// SubStateController通过SubState中的方法得到转移边
/// 2：每一个脚本都要预先挂载到SubStateController，通过addSubState加入到状态列表mapToSubState中，
/// 通过setInitialSubState设置初始状态
/// 3：在Update的时候，SubStateController会询问当前SubState是否转移，然后做出反应。
/// 假如需要转移，则会调用SubState的onSubStateExit执行善后工作，然后进行转移
/// </summary>
public class SubsceneController : MonoBehaviour
{
    ///所有SubState的句柄
    protected Hashtable mapToSubState = new Hashtable();

    ///当前状态
    protected string currentSubScene = null;

    //上一个状态
    protected string previousSubScene = null;

    ///是否设置最初状态
    protected bool mInitialized = false;

    ///@brief 为状态机添加子状态，以及子状态对应的subStateController
    ///@param name是子状态的名称，例如进入房间1，进入boss房间，钥匙获取战
    ///@param substate 是子状态的名称，对应的脚本应该提前挂载到StateController中
    public bool addSubscene(string name, string subSceneClassName)
    {
        if (!mapToSubState.ContainsKey(name))
        {
            //这里一定要注意要将subState冻结掉，不然一堆substate会出事
            ((GetComponent(subSceneClassName)) as Subscene).enabled = false;
            mapToSubState.Add(name, (GetComponent(subSceneClassName)));
            return true;
        }
        else
            return false;
    }


    ///@brief 为状态机选择最初始的子状态，以及子状态对应的subStateController，只能设置一次
    ///@param name是子状态的名称，例如进入房间1，进入boss房间，钥匙获取战
    ///@param substate 是子状态的名称，对应的脚本应该提前挂载到StateController中
    public void setInitialSubState(string name, string subState)
    {
        if (!mInitialized)
        {
            mInitialized = true;
            currentSubScene = name;
            ((mapToSubState[currentSubScene]) as Subscene).enabled = true;
            ((mapToSubState[currentSubScene]) as Subscene).onSubsceneInit();
        }
    }


    ///@brief 设置当前子状态,建议一般不要调用，除非是从头再来
    ///@param name是子状态的名称，例如进入房间1，进入boss房间，钥匙获取战
    ///@param substate 是子状态的名称，对应的脚本应该提前挂载到StateController中
    public void setCurrentSubState(string name, string subState)
    {
        if (currentSubScene != null)
        {
            ((mapToSubState[currentSubScene]) as Subscene).enabled = false;
        }
        if (name != null)
        {
            previousSubScene = currentSubScene;
            currentSubScene = name;
        }
        ((mapToSubState[currentSubScene]) as Subscene).enabled = true;
        ((mapToSubState[currentSubScene]) as Subscene).onSubsceneInit();
    }

    ///@brief update函数负责不断查询当前状态是否转移，因为在初始时所有的子状态脚本都被添加到了状态管理器上，并且将enable设置为false
    public void Update()
    {
        Subscene sub = ((mapToSubState[currentSubScene]) as Subscene);
        //Debug.Log(mapToSubState.Count);
        if (sub != null && sub.isTransitionTriggered())
        {
            Debug.Log(currentSubScene);
            //执行善后工作
            sub.onSubsceneDestory();
            //不再执行update操作控制逻辑
            sub.enabled = false;

            previousSubScene = currentSubScene;

            //更换subscene
            currentSubScene = sub.GetNextSubscene();
            //不要忘记设置enable
            ((mapToSubState[currentSubScene]) as Subscene).enabled = true;
            ((mapToSubState[currentSubScene]) as Subscene).onSubsceneInit();
        }
    }

    ///获取上一个状态
    public string previousSubscene {
        get {
            return previousSubScene;
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================================
//=========================    子状态类            =============================================
//==============================================================================================

/// <summary>
///  这个类抽象了一个子场景状态至少要实现的接口，包括：
///  1： 查询状态是否进行转移
///  2： 获取下一个状态的状态边
///  3： 此状态的初始化
///  4： 此状态转以后的操作，类似"后置状态"，例如Destrory一些敌人之类的
///  
///  具体的子类需要实现：
///  1: 通过onSubStateEnter实现相关元素的初始化，例如进入房间时，敌人的生成，敌人生成之后的脚本的挂载；
///  例如过场动画时，保存当前游戏的一些状态（人物位置，相机位置）
///  2: 根据场景内容，在update中实现相关条件的判断，决定转移边和是否转移，例如与门的交互
///  通过isSubStateChange告知SubStateController是否进行转移
///  3: 通过GetNextSubState告知SubStateController转移的下一个状态
///  4: 通过onSubStateExit实现一些善后工作，例如敌人物体的消除等
/// </summary>
/// 
namespace Room5Battle
{
    public class Subscene : MonoBehaviour
    {
        //============================================
        //=====   每一个子场景都至少实现以下接口  ======
        //============================================
        public const int c_InvalidStateId = -1;

        //@brief 返回此子场景是否将被转移
        virtual public bool isTransitionTriggered()
        {
            return true;
        }

        //@brief 返回下一个子场景
        virtual public int GetNextSubscene()
        {
            return c_InvalidStateId;
        }

        //@brief 子场景的初始化，可以在初始化阶段将所有元素的行为模式改为此状态下的逻辑
        virtual public void onSubsceneInit()
        {
        }

        //@brief 善后工作
        virtual public void onSubsceneDestory()
        {
        }


    }
}
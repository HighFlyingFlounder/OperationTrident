﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room5Battle
{
    //第一次进入房间后，
    public class Subscene_3min : Subscene
    {

        public override bool isTransitionTriggered()
        {
            return true;
        }

        //@brief 返回下一个子场景
        public override int GetNextSubscene()
        {
            return null;
        }

        //@brief 子场景的初始化，可以在初始化阶段将所有元素的行为模式改为此状态下的逻辑
        public override void onSubsceneInit()
        {
        }

        //@brief 善后工作
        public override void onSubsceneDestory()
        {
        }

    }
}
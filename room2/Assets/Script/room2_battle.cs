using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle {
    //小boss大战
    public class room2_battle :  Subscene{

        [SerializeField]
        protected Camera mCamera;

        [SerializeField]
        protected UnityEngine.Playables.PlayableDirector director;

        public override void notify(int i)
        {
            
        }

        public override bool isTransitionTriggered()
        {
            return false;
        }

        public override void onSubsceneDestory()
        {
            
        }

        public override void onSubsceneInit()
        {
           

        }

    }

}
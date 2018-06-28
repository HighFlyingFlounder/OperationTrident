using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Player {
    public class HealthController : MonoBehaviour {
        [Tooltip("最大血量")]
        [SerializeField] private int MaxHP;

        //当前是否存活
        public bool IsAlive { get; private set; }
        //当前血量
        public int CurrentHP { get; private set; }


        // Use this for initialization
        void Start() {
            //初始化当前血量
            CurrentHP = MaxHP;
        }

        public void TakeDamage(int damage) {
            int health = CurrentHP - damage;

            if(health > 0) {

            }
        }

        //执行受伤时的效果，参数是受伤时受到的力
        public void Hurt(float force, Vector3 direction) {

        }

        public void AddHP(int heal) {
            throw new System.NotImplementedException();
        }

    }
}

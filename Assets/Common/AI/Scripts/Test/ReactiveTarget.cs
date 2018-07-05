using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class ReactiveTarget : MonoBehaviour, AIReacitiveInterface
    {
        bool _isParalyzed;

        [SerializeField]
        int HP;

        int _currentHP;
        float _EMPEffectTime;


        public bool IsParalyzed
        {
            get
            {
                return _isParalyzed;
            }
        }

        public bool IsAlive
        {
            get
            {
                return _currentHP > 0;
            }
        }

        public float HPPercentage
        {
            get
            {
                return (float)_currentHP / HP;
            }
        }

        private void Awake()
        {
            _currentHP = HP;
            _isParalyzed = false;
            _EMPEffectTime = 0;
        }

        private void Update()
        {
            _EMPEffectTime -= Time.deltaTime;
            if (_EMPEffectTime < 0)
            {
                _isParalyzed = false;
                _EMPEffectTime = 0;
            }
        }

        public void OnEMP(float effectTime)
        {
            if (!IsAlive)
                return;

            _isParalyzed = true;
            _EMPEffectTime += effectTime;
        }

        public void OnHit(int damage)
        {
            if (!IsAlive)
                return;

            _currentHP -= damage;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace OperationTrident.Common.AI
{
    public abstract class AIAgent : MonoBehaviour
    {
        public ScriptableObject AIFSMAsset = null;
        public AIActionController ActionController = null;
        public ReactiveTarget AIReactiveTarget = null;

        protected AIFSM FSM = new AIFSM();
        Transform _target;
        Vector3 _targetPosition;
        bool _isParalyzed = false;
        public float fsmUpdateTime = 0.5f;
        float _time = 0;

        public virtual Vector3[] PatrolLocations { get; set; }
        public virtual int PatrolStartLocationIndex { get; set; }
        public virtual NavMeshAgent PathfindingAgent { get; set; }
        public virtual AICamera Camera { get; set; }
        public virtual float CameraHorizontalFOV { get; set; }
        public virtual float CameraVerticalFOV { get; set; }
        public virtual float CameraSightDistance { get; set; }
        public virtual float AttackPrecisionAngle { get; set; }
        public virtual float AttackPrecisionRadius { get; set; }
        public virtual Transform Center { get; set; }
        public virtual float DepressionAngle { get; set; }
        public virtual float DetectRangeMax { get; set; }

        AIFSMData FSMData
        {
            get
            {
                if (AIFSMAsset != null)
                    return AIFSMAsset as AIFSMData;
                Debug.Log("没有设置AIFSM");
                throw new System.NotImplementedException();
            }
        }

        public Transform Target
        {
            get
            {
                return _target;
            }

            set
            {
                _target = value;
            }
        }

        public Vector3 TargetPosition
        {
            get
            {
                return _targetPosition;
            }

            set
            {
                _targetPosition = value;
            }
        }

        public AIReacitiveInterface ReactiveTarget
        {
            get
            {
                return AIReactiveTarget as AIReacitiveInterface;
            }
        }

        // Use this for initialization
        protected void Start()
        {
            FSM.FSMData = FSMData;
            FSM.Init(this.gameObject);
        }

        // Update is called once per frame
        protected void Update()
        {
            // AI已经死亡
            if (!ReactiveTarget.IsAlive)
            {
                return;
            }

            // AI第一次进入麻痹状态，暂停AI动作
            if (ReactiveTarget.IsParalyzed && !_isParalyzed)
            {
                ActionController.RPC(ActionController.StopAllAction);
                _isParalyzed = true;
                return;
            }
            // 还处于麻痹状态中
            else if (ReactiveTarget.IsParalyzed)
            {
                return;
            }
            // 解除麻痹状态
            else if (!ReactiveTarget.IsParalyzed)
            {
                _isParalyzed = false;
            }

            _time += Time.deltaTime;
            if (_time > fsmUpdateTime)
            {
                _time = 0;
                FSM.Update();
            }
        }

        public abstract void SetInitParams(AIAgentInitParams initParams);

    }

    [System.Serializable]
    public class AIAgentInitParams { }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OperationTrident.Common.UI
{
    public abstract class UIBase : MonoBehaviour
    { 
        protected List<GameObject> tabSelectFields = new List<GameObject>();
        protected GameObject firstSelectField = null;

        protected UnityEngine.EventSystems.EventSystem system;

        // 判断是不是第一次初始化
        bool _firstInit;

        protected bool IsFirstInit
        {
            get
            {
                if (_firstInit)
                {
                    _firstInit = false;
                    return true;
                }
                return false;
            }
        }

        protected void Awake()
        {
            system = UnityEngine.EventSystems.EventSystem.current;
        }

        protected void OnEnable()
        {
            _firstInit = true;
        }

        protected void Update()
        {
            if (IsFirstInit)
            {
                FirstInit();
            }

            if(tabSelectFields.Count > 0)
            {
                if (system.currentSelectedGameObject != null && Input.GetKeyDown(KeyCode.Tab))
                {
                    GameObject selectedObj = system.currentSelectedGameObject;
                    int selectIndex = tabSelectFields.IndexOf(selectedObj);
                    selectIndex = (selectIndex + 1) % tabSelectFields.Count;
                    system.SetSelectedGameObject(tabSelectFields[selectIndex], new BaseEventData(system));
                }
                else if (Input.GetKeyDown(KeyCode.Tab))
                {
                    SelectFirstField();
                }
            }
        }

        protected virtual void FirstInit(){}

        public virtual void Open()
		{
			this.gameObject.SetActive(true);
		}

		public virtual void Close()
		{
			this.gameObject.SetActive(false);
		}

        protected void SelectFirstField()
        {
            if(firstSelectField != null)
                system.SetSelectedGameObject(firstSelectField, new BaseEventData(system));
        }
    }
}
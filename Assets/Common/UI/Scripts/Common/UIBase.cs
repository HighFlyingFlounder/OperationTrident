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

        protected void Awake()
        {
            system = UnityEngine.EventSystems.EventSystem.current;
        }
        protected void Update()
        {
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

        public virtual void Show()
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.UI
{
    public class UIManager : MonoBehaviour
    {
        protected Stack<GameObject> UIStack = new Stack<GameObject>();

        public void Open(GameObject UIPrefab)
        {
            if (GameObject.Find(UIPrefab.name) != null)
                return;

            HideCurrent();

            UIStack.Push(Instantiate(UIPrefab));
            UIStack.Peek().GetComponent<UIBase>().Open();
            UIStack.Peek().name = UIPrefab.name;
            DontDestroyOnLoad(UIStack.Peek());
        }

        public void CloseCurrent()
        {
            UIStack.Peek().GetComponent<UIBase>().Close();
            Destroy(UIStack.Pop());

            ShowLast();
        }

        public void ShowLast()
        {
            if (UIStack.Count != 0)
                UIStack.Peek().SetActive(true);
        }

        public void HideCurrent()
        {
            if (UIStack.Count != 0)
                UIStack.Peek().SetActive(false);
        }
    }
}
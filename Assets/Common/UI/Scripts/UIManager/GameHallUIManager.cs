using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.UI
{
    public class GameHallUIManager : Singleton<GameHallUIManager>
    {
        static Stack<GameObject> UIStack = new Stack<GameObject>();

        public void Open(GameObject UIPrefab)
        {
            if(GameObject.Find(UIPrefab.name) != null)
                return;

            if (UIStack.Count != 0)
                UIStack.Peek().SetActive(false);

            UIStack.Push(Instantiate(UIPrefab));
            UIStack.Peek().GetComponent<UIBase>().Show();
            UIStack.Peek().name = UIPrefab.name;
            DontDestroyOnLoad(UIStack.Peek());
        }
        public void Show()
        {
            if (UIStack.Count != 0)
                UIStack.Peek().SetActive(true);
        }

        public void Hide()
        {
            if (UIStack.Count != 0)
                UIStack.Peek().SetActive(false);
        }

        public void CloseCurrent()
        {
            UIStack.Peek().GetComponent<UIBase>().Close();
            Destroy(UIStack.Pop());

            if (UIStack.Count != 0)
                UIStack.Peek().SetActive(true);
        }
    }
}

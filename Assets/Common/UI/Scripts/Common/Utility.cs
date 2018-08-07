using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OperationTrident.Common.UI
{
    public class Utility
    {
        static public void ToUpperCase(InputField input)
        {
            input.text = input.text.ToUpper();
        }

        static public void ValidateNum(InputField input, int max)
        {
            string currInput = input.text;
            if (currInput == "-")
            {
                input.text = "";
                return;
            }

            if (currInput == "")
                return;

            if (int.Parse(currInput) > max)
            {
                input.text = currInput.Substring(0, currInput.Length - 1);
                return;
            }

            input.text = int.Parse(currInput).ToString();
        }

        static public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
        }

        static public void DeleteAllChildren(GameObject go)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Object.Destroy(go.transform.GetChild(i).gameObject);
            }
        }

        static public void EnableButton(Button button)
        {
            button.interactable = true;
            button.transform.GetComponentInChildren<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        static public void DisableButton(Button button)
        {
            button.interactable = false;
            button.transform.GetComponentInChildren<Text>().color = new Color(1.0f, 1.0f, 1.0f, 0.27f);
        }
    }
}
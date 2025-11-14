using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogOption : MonoBehaviour
{
    private Button button;
    private TMP_Text label;

    public bool Valid()
    {
        return button && label;
    }

    public void SetUp(Action onClick , string text)
    {
        label.text = text;
        //Clear all listeners
        button.onClick.RemoveAllListeners();

        //Add listener
        button.onClick.AddListener(() => {
            onClick.Invoke();
        });
    }
}
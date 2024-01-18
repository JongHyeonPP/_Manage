using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchBtnFocus : MonoBehaviour
{
    public int focusingIndex = 0;
    public List<Button> btnList;
    public bool isFocusActive = false;
    public void onClick_Func(int input)
    {
        swapFilter();

        void swapFilter()
        {
            int nextFocusingIndex = input;
            setBtnActive(btnList[focusingIndex], !isFocusActive);
            focusingIndex = input; 
            setBtnActive(btnList[nextFocusingIndex], isFocusActive);
        }
    }

    void setBtnActive(Button target, bool isActive)
    {
        if (isFocusActive)
            target.gameObject.SetActive(!isActive);
        else
            target.enabled = isActive;
    }
}

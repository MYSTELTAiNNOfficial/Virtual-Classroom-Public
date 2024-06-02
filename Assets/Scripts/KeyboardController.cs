using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    [SerializeField] GameObject inputTMP;

    string stringTemp;

    public void InputChar(string character)
    {
        stringTemp += character;
        inputTMP.GetComponent<TMP_Text>().text = stringTemp;
    }

    public void Backspace()
    {
        stringTemp = stringTemp.Remove(stringTemp.Length - 1);
        inputTMP.GetComponent <TMP_Text>().text = stringTemp;
    }
}

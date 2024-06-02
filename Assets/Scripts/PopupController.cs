using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmpWarning;
    Coroutine hide1;

    public void SetWarningPopup(string text)
    {
        if (hide1 != null)
        {
            StopCoroutine(hide1);
        }
        tmpWarning.text = text;
        gameObject.transform.GetComponent<CanvasGroup>().alpha = 1;
        gameObject.transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
        gameObject.transform.GetComponent<CanvasGroup>().interactable = true;
        hide1 = StartCoroutine(HideUI(gameObject, 5f));
    }

    //Reference: https://stackoverflow.com/a/30306585
    IEnumerator HideUI(GameObject guiParentCanvas, float secondsToWait, bool show = false)
    {
        yield return new WaitForSeconds(secondsToWait);
        if (show)
        {
            guiParentCanvas.transform.GetComponent<CanvasGroup>().alpha = 1;
            guiParentCanvas.transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
            guiParentCanvas.transform.GetComponent<CanvasGroup>().interactable = true;
        }
        else
        {
            guiParentCanvas.transform.GetComponent<CanvasGroup>().alpha = 0;
            guiParentCanvas.transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
            guiParentCanvas.transform.GetComponent<CanvasGroup>().interactable = false;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class InfoPart
{
    public string title;
    public string description;
    public GameObject partGroup;
}

public class ModelObjectController : MonoBehaviour
{
    [SerializeField] Animator gumsMouthAnim;
    [SerializeField] GameObject TMPPartInformation;
    [SerializeField] GameObject TMPPartInformation2;
    [SerializeField] List<InfoPart> partList;
    [SerializeField] LineController lineController;

    int indexSelectedPart;
    bool isAssesment = false;

    int currentCharacterIndex;
    int currentCharacterIndex2;
    Coroutine typewritingEffect;
    Coroutine typewritingEffect2;
    WaitForSeconds typeDelay;
    [SerializeField] float characterPerSecond;

    void Awake()
    {
        typeDelay = new WaitForSeconds(1/characterPerSecond);
    }

    public void SelectPart(int i)
    {
        indexSelectedPart = i;
        partList[i].partGroup.gameObject.SetActive(true);
        gumsMouthAnim.Play("decrease_opacity");
        lineController.SetTargetforLine(partList[i].partGroup);
        lineController.SetIsPoint3EnabledState(true);
        TMPPartInformation.SetActive(true);
        TMPPartInformation2.SetActive(true);
        AnimateText("<size=150%>" + partList[i].title + "<br><size=100%>" + partList[i].description);
    }

    public void RestoreState()
    {
        partList[indexSelectedPart].partGroup.gameObject.SetActive(false);
        gumsMouthAnim.Play("increase_opacity");
        indexSelectedPart = -1;
        lineController.RemoveTarget();
        TMPPartInformation.GetComponent<TMP_Text>().text = "";
        TMPPartInformation.SetActive(false);
        TMPPartInformation2.GetComponent<TMP_Text>().text = "";
        TMPPartInformation2.SetActive(false);
        if (typewritingEffect != null || typewritingEffect2 != null)
        {
            StopCoroutine(typewritingEffect);
            StopCoroutine(typewritingEffect2);
        }
    }

    public void AnimateText(string text)
    {
        if (typewritingEffect != null || typewritingEffect2 != null)
        {
            StopCoroutine(typewritingEffect);
            StopCoroutine(typewritingEffect2);
        }

        if (isAssesment)
        {
            TMPPartInformation.GetComponent<TMP_Text>().text = "<size=200%>???";
        }
        else
        {
            TMPPartInformation.GetComponent<TMP_Text>().text = text;
        }
        
        TMPPartInformation.GetComponent<TMP_Text>().maxVisibleCharacters = 0;
        TMPPartInformation2.GetComponent<TMP_Text>().text = text;
        TMPPartInformation2.GetComponent<TMP_Text>().maxVisibleCharacters = 0;
        currentCharacterIndex = 0;
        currentCharacterIndex2 = 0;
        typewritingEffect = StartCoroutine(Typing());
        typewritingEffect2 = StartCoroutine(Typing2());
    }

    public void SetAssesmentState() { isAssesment = !isAssesment; }
    public bool GetAssesmentState() { return isAssesment; }

    IEnumerator Typing()
    {
        //Reference: https://www.youtube.com/watch?v=UR_Rh0c4gbY       

        while (currentCharacterIndex < TMPPartInformation.GetComponent<TMP_Text>().textInfo.characterCount + 1)
        {
            TMPPartInformation.GetComponent<TMP_Text>().maxVisibleCharacters++;

            yield return typeDelay;

            currentCharacterIndex++;
        }
    }
    IEnumerator Typing2()
    {
        //Reference: https://www.youtube.com/watch?v=UR_Rh0c4gbY       

        while (currentCharacterIndex2 < TMPPartInformation2.GetComponent<TMP_Text>().textInfo.characterCount + 1)
        {
            TMPPartInformation2.GetComponent<TMP_Text>().maxVisibleCharacters++;

            yield return typeDelay;

            currentCharacterIndex2++;
        }
    }
}

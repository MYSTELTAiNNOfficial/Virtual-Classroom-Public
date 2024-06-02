using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;


[Serializable]
public class InfoPartOnline
{
    public string title;
    public string description;
    public GameObject partGroup;
}

public class ModelObjectControllerOnline : NetworkBehaviour
{
    [SerializeField] Animator gumsMouthAnim;
    [SerializeField] GameObject TMPPartInformation;
    [SerializeField] GameObject TMPPartInformation2;
    [SerializeField] List<InfoPartOnline> infoPartOnline;
    [SerializeField] LineController lineController;

    bool isAssesment2 = false;
    bool isRestricted2 = true;
    NetworkVariable<int> indexSelectedPart = new NetworkVariable<int>();
    NetworkVariable<bool> isAssesment = new NetworkVariable<bool>();
    NetworkVariable<bool> isRestricted = new NetworkVariable<bool>(true);
    NetworkVariable<FixedString512Bytes> titlePartTemp = new NetworkVariable<FixedString512Bytes>();
    NetworkVariable<FixedString512Bytes> descPartTemp = new NetworkVariable<FixedString512Bytes>();

    int currentCharacterIndex;
    int currentCharacterIndex2;
    Coroutine typewritingEffect;
    Coroutine typewritingEffect2;
    WaitForSeconds typeDelay;
    [SerializeField] float characterPerSecond;

    void Start()
    {
        typeDelay = new WaitForSeconds(1/characterPerSecond);
        indexSelectedPart.OnValueChanged += OnIndexSelectValueChange;
        isAssesment.OnValueChanged += OnAssesmentStateChange;
        isRestricted.OnValueChanged += OnRestrictedStateChange;
        titlePartTemp.OnValueChanged += OnTitlePartTempChange;
        descPartTemp.OnValueChanged += OnTitlePartTempChange;
    }

    private void OnTitlePartTempChange(FixedString512Bytes previousValue, FixedString512Bytes newValue)
    {
        if (previousValue != newValue && newValue != "")
        {
            Debug.Log("Prev Val: " + previousValue.ToString());
            Debug.Log("New Val: " + newValue.ToString());
            AnimateText("<size=150%>" + titlePartTemp.Value.ToString() + "<br><size=100%>" + descPartTemp.Value.ToString());
        }
    }


    private void OnIndexSelectValueChange(int previousValue, int newValue)
    {
        if (newValue != -1)
        {
            infoPartOnline[newValue].partGroup.SetActive(true);
            gumsMouthAnim.Play("decrease_opacity");
            lineController.SetIsPoint3EnabledState(IsHost);
            lineController.SetTargetforLine(infoPartOnline[newValue].partGroup);
            TMPPartInformation.SetActive(true);
            TMPPartInformation2.SetActive(true);
            SetTitleAndDescTextServerRpc(infoPartOnline[newValue].title, infoPartOnline[newValue].description);
        }
        if (newValue == -1)
        {
            Debug.Log("Prev Val: " + previousValue.ToString());
            infoPartOnline[previousValue].partGroup.SetActive(false);
            gumsMouthAnim.Play("increase_opacity");
            lineController.RemoveTarget();
            TMPPartInformation.GetComponent<TMP_Text>().text = "";
            TMPPartInformation.SetActive(false);
            TMPPartInformation2.GetComponent<TMP_Text>().text = "";
            TMPPartInformation2.SetActive(false);
            SetTitleAndDescTextServerRpc("", "");
            if (typewritingEffect != null || typewritingEffect2 != null)
            {
                StopCoroutine(typewritingEffect);
                StopCoroutine(typewritingEffect2);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetTitleAndDescTextServerRpc(string title, string desc)
    {
        titlePartTemp.Value = title;
        descPartTemp.Value = desc;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SelectPartServerRpc(int i) { indexSelectedPart.Value = i; }

    [ServerRpc(RequireOwnership = false)]
    public void RestoreStateServerRpc() { indexSelectedPart.Value = -1; }
    
    public void SelectPart(int i) { SelectPartServerRpc(i); }
    
    public void RestoreState() { RestoreStateServerRpc(); }

    //Assessment state stuff
    //START
    private void OnAssesmentStateChange(bool previousValue, bool newValue) { SetAssesmentState(newValue); }

    private void SetAssesmentState(bool newValue) { isAssesment2 = newValue; }

    [ServerRpc]
    public void ToggleSetAssesmentStateServerRpc() { isAssesment.Value = !isAssesment.Value; }

    public void ToggleSetAssesmentState() { ToggleSetAssesmentStateServerRpc(); }

    public bool GetAssessmentState() { return isAssesment2; }
    //END

    //Restricted state stuff
    //START
    private void OnRestrictedStateChange(bool previousValue, bool newValue) { SetRestrictedState(newValue); }

    private void SetRestrictedState(bool newValue) { isRestricted2 = newValue; }

    [ServerRpc]
    public void ToggleSetRestrictedStateServerRpc() { isRestricted.Value = !isRestricted.Value; }

    public void ToggleSetRestrictedState() {  ToggleSetRestrictedStateServerRpc(); }

    public bool GetRestrictedState() { return isRestricted2; }
    //END

    public void AnimateText(string text)
    {
        if (typewritingEffect != null)
        {
            StopCoroutine(typewritingEffect);
        }

        if (isAssesment2)
        {
            TMPPartInformation.GetComponent<TMP_Text>().text = "<size=200%>???";
        }
        else
        {
            TMPPartInformation.GetComponent<TMP_Text>().text = text;
        }
        
        TMPPartInformation.GetComponent<TMP_Text>().maxVisibleCharacters = 0;
        currentCharacterIndex = 0;
        typewritingEffect = StartCoroutine(Typing());

        if (IsHost)
        {
            if (typewritingEffect2 != null)
            {
                StopCoroutine(typewritingEffect2);
            }

            TMPPartInformation2.GetComponent<TMP_Text>().text = text;
            TMPPartInformation2.GetComponent<TMP_Text>().maxVisibleCharacters = 0;
            currentCharacterIndex2 = 0;
            typewritingEffect2 = StartCoroutine(Typing2());
        }
    }

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

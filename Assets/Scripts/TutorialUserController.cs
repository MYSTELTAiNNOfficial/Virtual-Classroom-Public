using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using static Unity.Burst.Intrinsics.X86;

public class TutorialUserController : MonoBehaviour
{
    [SerializeField] string[] texts;
    [SerializeField] GameObject textTutorial;
    [SerializeField] InputActionProperty inputForNextText;
    [SerializeField] Button tutorialButton;


    bool isCurrentTextDone;
    bool isGreetTextDone;
    int textIndex = 0;
    int currentCharacterIndex;
    Coroutine typewritingEffect;
    WaitForSeconds typeDelay;
    [SerializeField] float characterPerSecond;

    // Start is called before the first frame update
    void Start()
    {
        typeDelay = new WaitForSeconds(1 / characterPerSecond);
        AnimateText(texts[textIndex]);
        if (!PlayerPrefs.HasKey("isFirstTime") || PlayerPrefs.GetString("isFirstTime") == "true")
        {
            PlayerPrefs.SetString("isFirstTime", "true");
        }
        else if (PlayerPrefs.GetString("isFirstTime") == "false")
        {
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCharacterIndex == textTutorial.GetComponent<TMP_Text>().textInfo.characterCount + 1 && !isCurrentTextDone)
        {
            isCurrentTextDone = true;
        }
        
        if (inputForNextText.action.WasPressedThisFrame())
        {
            if (isCurrentTextDone && textIndex != (texts.Length - 1))
            {
                textIndex++;
                AnimateText(texts[textIndex]);
                isCurrentTextDone = false;
            }
            if (isCurrentTextDone && inputForNextText.action.WasPressedThisFrame() && textIndex == (texts.Length - 1))
            {
                textTutorial.SetActive(false);
            }
        }
    }

    public void ChangeToRandomColor()
    {
        tutorialButton.GetComponent<Image>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    public void MoveToMainMenuScene()
    {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        PlayerPrefs.SetString("isFirstTime", "false");
    }

    public void AnimateText(string text)
    {
        if (typewritingEffect != null)
        {
            StopCoroutine(typewritingEffect);
        }
        textTutorial.GetComponent<TMP_Text>().text = text;
        textTutorial.GetComponent<TMP_Text>().maxVisibleCharacters = 0;
        currentCharacterIndex = 0;
        typewritingEffect = StartCoroutine(Typing());
    }

    IEnumerator Typing()
    {
        //Reference: https://www.youtube.com/watch?v=UR_Rh0c4gbY       

        while (currentCharacterIndex < textTutorial.GetComponent<TMP_Text>().textInfo.characterCount + 1)
        {
            textTutorial.GetComponent<TMP_Text>().maxVisibleCharacters++;

            yield return typeDelay;

            currentCharacterIndex++;
        }
    }
}

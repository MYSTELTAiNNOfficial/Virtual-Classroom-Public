using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionUIController : MonoBehaviour
{
    [SerializeField] GameObject disableUiCameraStateTMP;
    [SerializeField] Animator anim;
    [SerializeField] GameObject oldNameTextTMP;
    [SerializeField] GameObject newNameInputTMP;
    [SerializeField] int limitTextToChangeAlignment;

    UserControllerOffline ucOffline;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            anim.Play("button_opened");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var otherCanvas = GetComponentsInChildren<CanvasGroup>();

            foreach (var c in otherCanvas)
            {
                if (c.name == "ChangeNamePanel" || c.name == "OptionPanel")
                {
                    c.alpha = 0;
                    c.blocksRaycasts = false;
                    c.interactable = false;
                }
            }
            anim.enabled = true;
            anim.Play("button_closed");
        }
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey("isCameraUiEnabled"))
        {
            PlayerPrefs.SetString("isCameraUiEnabled", "True");
        }

        ucOffline = FindAnyObjectByType<UserControllerOffline>();
        disableUiCameraStateTMP.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("isCameraUiEnabled");
    }
    // Update is called once per frame
    void Update()
    {
        SetOldNameTextToCurrentName();
        CheckLimitTextInput();
    }

    public void SetDisableCameraUiOption()
    {
        if (ucOffline == null)
        {
        ucOffline = FindAnyObjectByType<UserControllerOffline>();
        }
        if (PlayerPrefs.GetString("isCameraUiEnabled") == "False")
        {
            PlayerPrefs.SetString("isCameraUiEnabled", "True");
            disableUiCameraStateTMP.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("isCameraUiEnabled");
        }
        else if (PlayerPrefs.GetString("isCameraUiEnabled") == "True")
        {
            PlayerPrefs.SetString("isCameraUiEnabled", "False");
            disableUiCameraStateTMP.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("isCameraUiEnabled");
        }
        ucOffline.SetVisibilityCameraUI();
    }

    private void SetOldNameTextToCurrentName()
    {
        oldNameTextTMP.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("userName");
    }

    private void CheckLimitTextInput()
    {
        int textCount = newNameInputTMP.GetComponent<TMP_Text>().textInfo.characterCount;
        if (textCount > limitTextToChangeAlignment)
        {
            newNameInputTMP.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
        }
        if (textCount <= limitTextToChangeAlignment)
        {
            newNameInputTMP.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
        }
    }

    public void SetNewName()
    {
        string newName = newNameInputTMP.GetComponent<TextMeshProUGUI>().text;
        if (newName != null && newName != " " && newName != "")
        {
            PlayerPrefs.SetString("userName", newName);
        }

        if (ucOffline == null)
        {
            ucOffline = FindAnyObjectByType<UserControllerOffline>();
        }
        else
        {
            ucOffline.SetNameTag();
        }

        newNameInputTMP.GetComponent<TextMeshProUGUI>().text = "";
    }
}

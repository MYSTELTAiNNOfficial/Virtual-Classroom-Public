using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class UserControllerOffline : MonoBehaviour
{
    //Other information
    string currentScene;
    bool isPaused = false;
    bool isEnteringPointerRoom = false;

    //Player information
    string userName;
    string role;

    //GameObjects on Player
    [SerializeField] GameObject nameTag;
    [SerializeField] GameObject roleTag;
    [SerializeField] GameObject classroomMenuCanvas;
    [SerializeField] GameObject mainMenuMenuCanvas;
    [SerializeField] GameObject pointerMenuCanvas;
    [SerializeField] GameObject cameraUICanvas;
    [SerializeField] GameObject cameraUIHostCanvas;

    //Script on Player
    [SerializeField] DynamicMoveProvider moveProvider;
    [SerializeField] ActionBasedContinuousTurnProvider actionBasedContinuousTurn;
    [SerializeField] XRPokeInteractor rightPokeInteract;
    [SerializeField] XRPokeInteractor leftPokeInteract;
    [SerializeField] XRDirectInteractorCustom rDirectInteractorCustom;
    [SerializeField] XRDirectInteractorCustom lDirectInteractorCustom;

    //For input
    [SerializeField] InputActionProperty inputForMenu;
    [SerializeField] InputActionProperty inputForOpenPointerMenu;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PointerMenuTrigger")
        {
            isEnteringPointerRoom = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "PointerMenuTrigger")
        {
            isEnteringPointerRoom = false;
            if (pointerMenuCanvas.activeSelf)
            {
                OpenPointerMenu();
            }
        }
    }

    private void Awake()
    {
        //Set currentScene
        currentScene = SceneManager.GetActiveScene().name.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetNameTag();
        SetRoleTag();
        SetVisibilityCameraUI();

    }

    // Update is called once per frame
    void Update()
    {
        //Call Menu or Pause
        if (inputForMenu.action.WasPressedThisFrame())
        {
            Debug.Log("Pressed!");
            OpenPauseMenu();
        }

        //Call Pointer Menu
        if (inputForOpenPointerMenu.action.WasPressedThisFrame() && !isPaused && isEnteringPointerRoom)
        {
            OpenPointerMenu();
        }
    }

    public void SetVisibilityCameraUI()
    {
        cameraUICanvas.SetActive(bool.Parse(PlayerPrefs.GetString("isCameraUiEnabled")));
        if (SceneManager.GetActiveScene().ToString() == "Main Menu")
        {
            cameraUICanvas.SetActive(bool.Parse(PlayerPrefs.GetString("isCameraUiEnabled")));
            cameraUIHostCanvas.SetActive(false);
        }
        else if (SceneManager.GetActiveScene().ToString() == "Classroom Offline")
        {
            cameraUIHostCanvas.SetActive(bool.Parse(PlayerPrefs.GetString("isCameraUiEnabled")));
            cameraUICanvas.SetActive(false);
        }

    }

    public void SetNameTag()
    {
        //Set Nametag
        if (PlayerPrefs.HasKey("userName"))
        {
            userName = PlayerPrefs.GetString("userName");
        }
        else
        {
            userName = "User-";
            for (int i = 0; i < 10; i++)
            {
                userName += Random.Range(0, 10).ToString();
            }
            PlayerPrefs.SetString("userName", userName);
        }

        nameTag.GetComponent<TextMeshProUGUI>().text = userName;
    }

    public void SetRoleTag()
    {
        if (currentScene == "Main Menu")
        {
            PlayerPrefs.DeleteKey("role");
            role = "None";
            roleTag.GetComponent<TextMeshProUGUI>().text = "<< " + role + " >>";
        }
        else if (currentScene == "Classroom")
        {
            roleTag.GetComponent<TextMeshProUGUI>().text = "<< " + PlayerPrefs.GetString("role", "Lecturer") + " >>";
        }
    }

    public void OpenPauseMenu()
    {
        if (pointerMenuCanvas.activeSelf)
        {
            OpenPointerMenu();
        }

        isPaused = !isPaused;

        Debug.Log(rightPokeInteract.physicsLayerMask.value);
        Debug.Log(rDirectInteractorCustom.interactionLayers.value);

        if (rightPokeInteract.physicsLayerMask.value == -1 && leftPokeInteract.physicsLayerMask.value == -1)
        {
            leftPokeInteract.physicsLayerMask = 1 << 9;
            rightPokeInteract.physicsLayerMask = 1 << 9;
        }
        else
        {
            leftPokeInteract.physicsLayerMask = -1;
            rightPokeInteract.physicsLayerMask = -1;
        }

        if (rDirectInteractorCustom.interactionLayers.value == -1 && lDirectInteractorCustom.interactionLayers.value == -1)
        {
            rDirectInteractorCustom.interactionLayers = 0;
            lDirectInteractorCustom.interactionLayers = 0;
        }
        else
        {
            rDirectInteractorCustom.interactionLayers = -1;
            lDirectInteractorCustom.interactionLayers = -1;
        }

        if (currentScene == "Main Menu")
        {
            mainMenuMenuCanvas.SetActive(!mainMenuMenuCanvas.activeSelf);
        }
        else if (currentScene == "Classroom")
        {
            classroomMenuCanvas.SetActive(!classroomMenuCanvas.activeSelf);
        }
    }

    public void OpenPointerMenu()
    {
        pointerMenuCanvas.SetActive(!pointerMenuCanvas.activeSelf);
    }
}


using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class UserController : NetworkBehaviour
{
    //Other information
    string currentScene;
    bool isPaused = false;
    bool isEnteringPointerRoom = false;
    float nextPosUpdate;
    [SerializeField] GameObject playerSpawn;
    NetworkVariable<FixedString64Bytes> playerName = new NetworkVariable<FixedString64Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    NetworkVariable<FixedString64Bytes> playerRole = new NetworkVariable<FixedString64Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    //Player information
    string userName;

    //GameObjects on Player
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject nameTag;
    [SerializeField] GameObject roleTag;
    [SerializeField] GameObject classroomMenuCanvas;
    [SerializeField] GameObject mainMenuMenuCanvas;
    [SerializeField] GameObject pointerMenuCanvas;
    [SerializeField] GameObject chatMenuCanvas;
    [SerializeField] GameObject chatInput;
    [SerializeField] GameObject chatLog;
    [SerializeField] GameObject cameraUiCanvas;
    [SerializeField] GameObject cameraUiHostCanvas;
    [SerializeField] GameObject joinCodeCanvas;

    //Script on Player
    [SerializeField] Camera playerCamera;
    [SerializeField] TrackedPoseDriver trackedPoseDriver;
    [SerializeField] ActionBasedController[] controller;
    [SerializeField] DynamicMoveProviderClient moveProvider;
    [SerializeField] ActionBasedContinuousTurnProvider actionBasedContinuousTurn;
    [SerializeField] XRPokeInteractor rightPokeInteract;
    [SerializeField] XRPokeInteractor leftPokeInteract;
    [SerializeField] XRDirectInteractorCustom rDirectInteractorCustom;
    [SerializeField] XRDirectInteractorCustom lDirectInteractorCustom;

    //For input
    [SerializeField] InputActionProperty inputForMenu;
    [SerializeField] InputActionProperty inputForOpenPointerMenu;

    //Checking if host trigger the collider to open pointer menu
    //START
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PointerMenuTrigger" && IsHost)
        {
            isEnteringPointerRoom = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "PointerMenuTrigger" && IsHost)
        {
            isEnteringPointerRoom = false;
            if (pointerMenuCanvas.GetComponent<CanvasGroup>().alpha == 1)
            {
                OpenPointerMenu();
            }
        }
    }
    //END

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
        playerName.OnValueChanged += OnPlayerNameChanged;
        playerRole.OnValueChanged += OnPlayerRoleChanged;

        NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
        if (IsOwner || IsLocalPlayer)
        {
            SetNameTagServerRpc(PlayerPrefs.GetString("userName", "Unnamed Player").ToString());
            if (IsHost)
            {
                SetRoleTagServerRpc(PlayerPrefs.GetString("role", "Lecturer").ToString());
                cameraUiHostCanvas.SetActive(true);
            }
            else
            {
                SetRoleTagServerRpc(PlayerPrefs.GetString("role", "Student").ToString());
                cameraUiCanvas.SetActive(true);
            }
        }
        else
        {
            SetRoleTag(playerRole.Value.ToString());
            SetNameTag(playerName.Value.ToString());

        }
    }

    //Callback stuff
    //START
    private void OnClientConnectedCallback(ulong obj)
    {
        SetNameTagClientRpc();
        SetRoleTagClientRpc();
        DisableClientInput();

        SetJoinCodeText();
        SetCameraUiVisibility();
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId)
        {
            DisconnectFromServer();
        }
        else
        {
            Debug.Log(NetworkManager.Singleton.ConnectedClients.Count);
        }
    }
    //END

    // Update is called once per frame
    void Update()
    {
        currentScene = SceneManager.GetActiveScene().name.ToString();
        if (IsClient && (IsOwner || IsLocalPlayer))
        {
            //Auto fix pos y of the player
            if (gameObject.transform.position.y < 0)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0.1f, gameObject.transform.position.z);
            }

            //Call Pointer Menu
            if (inputForOpenPointerMenu.action.WasPressedThisFrame() && !isPaused && isEnteringPointerRoom && IsHost)
            {
                OpenPointerMenu();
            }

            //Call Menu or Pause
            if (inputForMenu.action.WasPressedThisFrame())
            {
                Debug.Log("Pressed!");
                OpenPauseMenu();

                Debug.Log("IsLocalPlayer " + IsLocalPlayer);
                Debug.Log("IsHost " + IsHost);
                Debug.Log("IsClient " + IsClient);
                Debug.Log("IsOwner " + IsOwner);
            }

            //Update local player pos for update positional audio
            if (Time.time > nextPosUpdate)
            {
                UpdatePlayerPos();
                nextPosUpdate += 0.3f;
            }

            //Reset XR Interaction Manager
            if (rDirectInteractorCustom.interactionManager == null || lDirectInteractorCustom.interactionManager == null)
            {
                XRInteractionManager interactionManager = GameObject.FindObjectOfType<XRInteractionManager>();

                rDirectInteractorCustom.interactionManager = interactionManager;
                lDirectInteractorCustom.interactionManager = interactionManager;
            }
        }

        UpdateExcludeTagForClient();
        DisableClientInput();
    }

    //Setting Name, Role, dll
    //START
    private void OnPlayerRoleChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        SetRoleTag(newValue.ToString());
    }

    private void OnPlayerNameChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        SetNameTag(newValue.ToString());
    }

    public void SetNameTag(string name)
    {
        nameTag.GetComponent<TextMeshProUGUI>().text = name;
    }

    public void SetRoleTag(string role)
    {
        roleTag.GetComponent<TextMeshProUGUI>().text = "<< " + role + " >>";
    }

    [ClientRpc]
    public void SetNameTagClientRpc()
    {
        if (IsClient && (IsOwner || IsLocalPlayer))
        {
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

            if (!IsHost)
            {
                rDirectInteractorCustom.excludeTag.Add("Anatomi");
                lDirectInteractorCustom.excludeTag.Add("Anatomi");
            }
        }
    }

    [ClientRpc]
    public void SetRoleTagClientRpc()
    {
        if (IsClient && (IsOwner || IsLocalPlayer))
        {
            if (IsHost)
            {
                PlayerPrefs.SetString("role", "Lecturer");
                roleTag.GetComponent<TextMeshProUGUI>().text = "<< " + PlayerPrefs.GetString("role", "???") + " >>";
            }
            else
            {
                PlayerPrefs.SetString("role", "Student");
                roleTag.GetComponent<TextMeshProUGUI>().text = "<< " + PlayerPrefs.GetString("role", "???") + " >>";
            }
        }
    }

    public void SetJoinCodeText()
    {
        joinCodeCanvas.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("currentCode");
    }

    public void SetCameraUiVisibility()
    {
        if (IsOwner)
        {
            if (IsHost)
            {
                cameraUiHostCanvas.SetActive(bool.Parse(PlayerPrefs.GetString("isCameraUiEnabled")));
            }
            else
            {
                cameraUiCanvas.SetActive(bool.Parse(PlayerPrefs.GetString("isCameraUiEnabled")));
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetNameTagServerRpc(string name)
    {
        playerName.Value = name;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetRoleTagServerRpc(string role)
    {
        playerRole.Value = role;
    }

    public void UpdateExcludeTagForClient()
    {
        if (!IsHost)
        {
            ModelObjectControllerOnline modelObjectController = FindObjectOfType<ModelObjectControllerOnline>();

            if (modelObjectController != null)
            {
                bool restrictState = modelObjectController.GetRestrictedState();
                if (restrictState)
                {
                    if (rDirectInteractorCustom.excludeTag.Contains("Anatomi"))
                    {
                        return;
                    }
                    else
                    {
                        rDirectInteractorCustom.excludeTag.Add("Anatomi");
                        lDirectInteractorCustom.excludeTag.Add("Anatomi");
                    }
                }
                else
                {
                    rDirectInteractorCustom.excludeTag.Remove("Anatomi");
                    lDirectInteractorCustom.excludeTag.Remove("Anatomi");
                }
            }
        }
    }
    //END

    //Disconnecting stuff
    //START
    public async void DisconnectFromServer()
    {
        if (IsClient && (IsLocalPlayer || IsOwner))
        {
            await VivoxService.Instance.LeaveChannelAsync(PlayerPrefs.GetString("currentCode"));
            await VivoxService.Instance.LogoutAsync();
            NetworkManager.Singleton.Shutdown();
            NetworkManager networkManager = GameObject.FindObjectOfType<NetworkManager>();
            Destroy(networkManager.gameObject);
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        }
    }
    //END

    //Pause menu stuff
    //START
    public void OpenPauseMenu()
    {
        if (IsClient && (IsOwner || IsLocalPlayer))
        {
            if (pointerMenuCanvas.GetComponent<CanvasGroup>().alpha == 1 && IsHost)
            {
                OpenPointerMenu();
            }

            if (bool.Parse(PlayerPrefs.GetString("isCameraUiEnabled")) == true)
            {
                if (IsHost)
                {
                    cameraUiHostCanvas.SetActive(!cameraUiHostCanvas.activeSelf);
                }
                else if (IsClient)
                {
                    cameraUiCanvas.SetActive(!cameraUiCanvas.activeSelf);
                }
            }

            isPaused = !isPaused;

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
            else if (currentScene == "Classroom Online")
            {
                classroomMenuCanvas.SetActive(!classroomMenuCanvas.activeSelf);
            }
        }
    }
    //END

    //Pointer menu stuff
    //START
    public void OpenPointerMenu()
    {
        if (IsHost)
        {
            if (pointerMenuCanvas.GetComponent<CanvasGroup>().alpha == 0)
            {
                pointerMenuCanvas.GetComponent<CanvasGroup>().alpha = 1;
                pointerMenuCanvas.GetComponent<CanvasGroup>().interactable = true;
                pointerMenuCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
            } 
            else if (pointerMenuCanvas.GetComponent<CanvasGroup>().alpha == 1)
            {
                pointerMenuCanvas.GetComponent<CanvasGroup>().alpha = 0;
                pointerMenuCanvas.GetComponent<CanvasGroup>().interactable = false;
                pointerMenuCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }
    }
    //END

    //Voice Chat stuff
    //START
    public void UpdatePlayerPos()
    {
        string currentClassCode = PlayerPrefs.GetString("currentCode");
        VivoxService.Instance.Set3DPosition(NetworkManager.LocalClient.PlayerObject.gameObject, currentClassCode);
    }
    //END

    //Reference: Dilmer Valecillos
    //Disable other client input on local player (or owner) side
    //START
    public void DisableClientInput()
    {
        if (IsClient && (!IsLocalPlayer || !IsOwner))
        {
            if (!IsHost)
            {
                pointerMenuCanvas.SetActive(false);
            }

            playerCamera.enabled = false;
            moveProvider.enableInputActions = false;
            moveProvider.enabled = false;
            actionBasedContinuousTurn.enabled = false;
            trackedPoseDriver.enabled = false;
            cameraUiCanvas.SetActive(false);
            cameraUiHostCanvas.SetActive(false);
            joinCodeCanvas.SetActive(false);

            foreach (ActionBasedController controllerTemp in controller)
            {
                controllerTemp.enableInputActions = false;
                controllerTemp.enableInputTracking = false;
                controllerTemp.enabled = false;
            }
        }
    }
    //END

    //Changing ownershp of the object when interacting
    //START
    public void OnSelectInteractable(SelectEnterEventArgs eventArgs)
    {
        if (IsLocalPlayer && (IsOwner || IsClient))
        {
            Debug.Log(eventArgs.interactableObject.transform.name);
            NetworkObject networkObject = eventArgs.interactableObject.transform.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                ReqGrabOwnershipServerRpc(OwnerClientId, networkObject);
            }

            //Pengecualian untuk interaksi dengan anatomi
            if (eventArgs.interactableObject.transform.name == "Anatomy")
            {
                NetworkObject no = GameObject.FindGameObjectWithTag("ModelGigi").transform.GetComponent<NetworkObject>();
                if (no != null)
                {
                    if (!IsHost)
                    {
                        ModelObjectControllerOnline modelObjectController = FindObjectOfType<ModelObjectControllerOnline>();
                        if (modelObjectController != null)
                        {
                            bool restrictState = modelObjectController.GetRestrictedState();
                            if (restrictState)
                            {
                                return;
                            }
                            else
                            {
                                ReqGrabOwnershipServerRpc(OwnerClientId, no);
                            }
                        }
                    }
                    else if (IsHost)
                    {
                        ReqGrabOwnershipServerRpc(OwnerClientId, no);
                    }
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReqGrabOwnershipServerRpc(ulong newOwnerCliendId, NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.ChangeOwnership(newOwnerCliendId);
        }
        else
        {
            Debug.Log("Unable to change ownership for " + newOwnerCliendId);
        }
    }
    //END
}

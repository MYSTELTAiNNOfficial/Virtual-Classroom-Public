using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Vivox;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkConnection : MonoBehaviour
{
    [SerializeField] string createdJoinCode;
    [SerializeField] int maxConnection;
    [SerializeField] UnityTransport unityTransport;

    PopupController popupController;

    private async void Awake()
    {
        if (GetComponent<NetworkManager>() != NetworkManager.Singleton)
        {
            Debug.Log("Destroying this clone", gameObject);
            Destroy(gameObject);
            await VivoxService.Instance.LogoutAsync();
            NetworkManager.Singleton.SetSingleton();
        }
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        await VivoxService.Instance.InitializeAsync();
        await VivoxService.Instance.LoginAsync();
    }

    public async void CreateClassroom()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
            createdJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            PlayerPrefs.SetString("currentCode", createdJoinCode);
            Debug.Log(createdJoinCode);

            unityTransport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes,
                allocation.Key, allocation.ConnectionData);

            NetworkManager.Singleton.StartHost();
            Channel3DProperties channel3DProperties = new Channel3DProperties();
            await VivoxService.Instance.JoinPositionalChannelAsync(createdJoinCode.ToString(), ChatCapability.AudioOnly, channel3DProperties);
            NetworkManager.Singleton.SceneManager.LoadScene("Classroom Online", LoadSceneMode.Single);
        }
        catch (System.Exception e)
        {
            popupController = FindObjectOfType<PopupController>();
            if (popupController != null)
            {
                popupController.SetWarningPopup(e.Message);
            }
            else
            {
                Debug.LogError(e.Message);
            }
        }
    }

    public async void JoinClassroom(string code)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(code);
            PlayerPrefs.SetString("currentCode", code);
            
            unityTransport.SetClientRelayData(joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port, joinAllocation.AllocationIdBytes,
                joinAllocation.Key, joinAllocation.ConnectionData, joinAllocation.HostConnectionData);

            if (VivoxService.Instance.ActiveChannels != null)
            {
                await VivoxService.Instance.LeaveAllChannelsAsync();
            }
            
            NetworkManager.Singleton.StartClient();
            Channel3DProperties channel3DProperties = new Channel3DProperties();
            await VivoxService.Instance.JoinPositionalChannelAsync(code, ChatCapability.AudioOnly, channel3DProperties);
        }
        catch(System.Exception e)
        {
            popupController = FindObjectOfType<PopupController>();

            if (popupController != null)
            {
                popupController.SetWarningPopup(e.Message);
            }
            else
            {
                Debug.LogError(e.Message);
            }
        }
    }

    public string ShowJoinCode()
    {
        return createdJoinCode;
    }
}

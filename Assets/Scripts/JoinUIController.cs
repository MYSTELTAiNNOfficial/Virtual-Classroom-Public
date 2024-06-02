using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinUIController : MonoBehaviour
{
    [SerializeField] GameObject inputTMP;

    public void SendCodeToNetConnect()
    {
        NetworkConnection netConnect = GameObject.FindObjectOfType<NetworkConnection>();

        netConnect.JoinClassroom(inputTMP.GetComponent<TMP_Text>().text);
    }
}

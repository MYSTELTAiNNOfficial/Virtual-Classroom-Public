using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class MainMenuButtonController : MonoBehaviour
{
    [SerializeField] GameObject inputJoinCodeUI;
    [SerializeField] GameObject createClassButton;

    Animator anim;
    XRPokeInteractor rightPokeInteract;
    XRPokeInteractor leftPokeInteract;

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
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("button_opened") || anim.GetCurrentAnimatorStateInfo(0).IsName("button2_closed"))
            {
                anim.Play("button_closed");
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("button2_opened"))
            {
                anim.Play("buttonall_closed");
            }
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();

        XRPokeInteractor[] pokeTemps = GameObject.FindObjectsByType<XRPokeInteractor>(FindObjectsSortMode.None);
        foreach (XRPokeInteractor pokeTemp in pokeTemps)
        {
            if (pokeTemp.gameObject.name == "Left Poke Interactor")
            {
                leftPokeInteract = pokeTemp;
            }
            else if (pokeTemp.gameObject.name == "Right Poke Interactor")
            {
                rightPokeInteract = pokeTemp;
            }
        }
    }

    public void OnCreateClicked()
    {
        NetworkConnection networkConnection = FindAnyObjectByType<NetworkConnection>();
        networkConnection.CreateClassroom();
    }

    public void OpenButtonForOnline()
    {
        anim.Play("button2_opened");
    }

    public void CloseButtonForOnline()
    {
        anim.Play("button2_closed");
    }

    public void MoveClassroomOffline()
    {
        SceneManager.LoadScene("Classroom", LoadSceneMode.Single);
    }

    public void ChangePokeLayerOnInteractJoinUI()
    {
        leftPokeInteract.physicsLayerMask = 1 << 10;
        rightPokeInteract.physicsLayerMask = 1 << 10;
    }

    public void ChangePokeLayerToDefault()
    {
        leftPokeInteract.physicsLayerMask = -1;
        rightPokeInteract.physicsLayerMask = -1;
    }

    public void DisableUIInputJoin()
    {
        inputJoinCodeUI.GetComponent<CanvasGroup>().alpha = 0;
        inputJoinCodeUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
        inputJoinCodeUI.GetComponent<CanvasGroup>().interactable = false;
    }
}

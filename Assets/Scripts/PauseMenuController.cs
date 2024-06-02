using System.Collections;
using System.Collections.Generic;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    UserController controller;
    UserControllerOffline controllerOffline;

    private void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<UserController>();
        controllerOffline = GameObject.FindGameObjectWithTag("Player").GetComponent<UserControllerOffline>();
    }

    public void ResumeButton()
    {
        if (controller == null)
        {
            controllerOffline.OpenPauseMenu();
        }
        if (controllerOffline == null)
        {
            controller.OpenPauseMenu();
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

    public async void ExitApp()
    {
        await VivoxService.Instance.LogoutAsync();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}

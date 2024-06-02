using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);

        if (PlayerPrefs.GetString("isFirstTime") == "false" && PlayerPrefs.HasKey("isFirstTime"))
        {
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        }
    }
}

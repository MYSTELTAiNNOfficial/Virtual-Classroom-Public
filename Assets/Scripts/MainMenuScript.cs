using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] GameObject playerSpawn;
    [SerializeField] GameObject player;

    private void Awake()
    {
        SummonPlayer();
    }

    private void FixedUpdate()
    {
        CheckPlayerPos();
    }

    void CheckPlayerPos()
    {
        //Check if player posY are below 0
        if (player.transform.position.y < 0)
        {
            //if below 0, move player to playerSpawn
            player.transform.position = playerSpawn.transform.position;
        }
    }

    void SummonPlayer()
    {
        Instantiate(player, playerSpawn.transform);
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.SetParent(null);
        player.transform.position = playerSpawn.transform.position;
    }
}

using UnityEngine;

public class ClassroomManager : MonoBehaviour
{
    [SerializeField] GameObject playerSpawn;
    [SerializeField] GameObject playerPrefab;

    GameObject playerTemp;

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
        //Check if player posY are above 0
        if (playerPrefab.transform.position.y < 0)
        {
            //if below 0, move player to playerSpawn
            playerPrefab.transform.position = playerSpawn.transform.position;
        }
    }

    void SummonPlayer()
    {
        Instantiate(playerPrefab, playerSpawn.transform);
        playerTemp = GameObject.FindGameObjectWithTag("Player");
        playerTemp.transform.parent = null;
        playerTemp.transform.position = playerSpawn.transform.position;
    }
}

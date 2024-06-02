using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MirrorCameraController : MonoBehaviour
{
    GameObject playerCamera;
    GameObject mirror;

    private void Start()
    {
        Init();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (playerCamera != null)
        {
            // Reference: https://www.youtube.com/watch?v=txF4t1qynyk
            Vector3 playerCameraPos = mirror.transform.InverseTransformPoint(playerCamera.transform.position);
            transform.position = new Vector3(transform.position.x, playerCamera.transform.position.y, transform.position.z);
            Vector3 mirrorMove = mirror.transform.TransformPoint(new Vector3(-playerCameraPos.x, playerCameraPos.y, playerCameraPos.z));
            gameObject.transform.LookAt(mirrorMove);
        }
        else
        {
            Init();
        }
    }

    void Init()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mirror = GameObject.FindGameObjectWithTag("Mirror");
    }
}

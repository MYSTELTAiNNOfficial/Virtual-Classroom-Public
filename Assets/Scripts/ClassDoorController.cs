using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassDoorController : MonoBehaviour
{
    [SerializeField] Animator animator;
    bool isOpen = false;

    public void InteractDoor()
    {
        Debug.Log("Interacted!");
        if (isOpen)
        {
            animator.Play("door_close");
            isOpen = false;
        } else if (!isOpen)
        {
            animator.Play("door_open");
            isOpen = true;
        }
    }
}

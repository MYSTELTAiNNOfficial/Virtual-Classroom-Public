using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(ActionBasedController))]
public class HandController : MonoBehaviour
{
    //Reference: https://www.youtube.com/watch?v=DxKWq7z4Xao

    ActionBasedController controller;
    public Hand hand;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ActionBasedController>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("selectAction: " + controller.selectActionValue.action.ReadValue<float>());
        Debug.Log("activateAction: " + controller.activateActionValue.action.ReadValue<float>());
        //hand.SetGrip(controller.selectActionValue.action.ReadValue<float>());
        //hand.SetTrigger(controller.activateActionValue.action.ReadValue<float>());
    }
}

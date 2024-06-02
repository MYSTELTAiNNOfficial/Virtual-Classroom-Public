using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Animator))]
public class HandOffline : MonoBehaviour
{
    //Reference: Valem Tutorial

    public InputActionProperty pokeAnimationAction;
    public InputActionProperty gripAnimationAction;
    public Animator handAnimator;
    public GameObject indexFingerCollider;
    public GameObject pokeInteractor;

    void Start()
    {
        handAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get the latest trigger and grip value
        float triggerValue = pokeAnimationAction.action.ReadValue<float>();
        float gripValue = gripAnimationAction.action.ReadValue<float>();

        handAnimator.SetFloat("Trigger", triggerValue);
        handAnimator.SetFloat("Grip", gripValue);

        if (triggerValue > 0.7f)
        {
            indexFingerCollider.SetActive(true);
            pokeInteractor.GetComponent<XRPokeInteractor>().enabled = true;
        }
        else
        {
            indexFingerCollider.SetActive(false);
            pokeInteractor.GetComponent<XRPokeInteractor>().enabled = false;
        }
    }
}

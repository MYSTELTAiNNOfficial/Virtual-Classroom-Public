using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRDirectInteractorCustom : XRDirectInteractor
{
    [Tooltip("Digunakan untuk mengexclude object dengan tag agar tidak bisa digrab atau diinteraksi")]
    public List<string> excludeTag = new List<string>();
    private string selectedGameObject;

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        if (excludeTag.Count != 0)
        {
            bool canGrabObject = false;
            foreach (string tag in excludeTag)
            {
                if (interactable.transform.tag == tag)
                {
                    canGrabObject = false;
                    break;
                }
                else
                {
                    canGrabObject = true;
                }
            }
            return base.CanSelect(interactable) && canGrabObject;
        }
        //Digunakan jika tidak ada tag yang ingin diexclude
        else
        {
            return base.CanSelect(interactable);
        }
    }

    public string GetSelectedGameObject()
    {
        return selectedGameObject;
    }

    public void AddExcludeTag(string value)
    {

    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        selectedGameObject = args.interactableObject.transform.name;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        selectedGameObject = null;
    }
}

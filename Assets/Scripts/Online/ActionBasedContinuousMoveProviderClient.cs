using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ActionBasedContinuousMoveProviderClient : ActionBasedContinuousMoveProvider
{
    [SerializeField] bool enableInputActions;

    protected override Vector2 ReadInput()
    {
        if (!enableInputActions)
        {
            return Vector2.zero;
        }
        return base.ReadInput();
    }
}

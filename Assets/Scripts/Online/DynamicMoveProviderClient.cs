using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class DynamicMoveProviderClient : DynamicMoveProvider
{
    public bool enableInputActions;

    protected override Vector2 ReadInput()
    {
        if (!enableInputActions)
        {
            return Vector2.zero;
        }
        return base.ReadInput();
    }
}

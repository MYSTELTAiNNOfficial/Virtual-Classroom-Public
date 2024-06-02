using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class NetworkAnimatorClient : NetworkAnimator
{
    //Reference: Unity Github & Valem Tutorial
    // Start is called before the first frame update

    protected override bool OnIsServerAuthoritative()
    {
        //return base.OnIsServerAuthoritative();
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class NetworkTransformClient : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        //Reference: Unity & Valem Tutorial
        //return base.OnIsServerAuthoritative();
        return false;
    }
}

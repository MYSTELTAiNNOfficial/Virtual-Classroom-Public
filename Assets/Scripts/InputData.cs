using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR;
public class InputData : NetworkBehaviour
{
    //Reference: https://www.youtube.com/watch?v=Kh_94glqO-0

    public InputDevice leftController;
    public InputDevice rightController;
    public InputDevice headController;

    //public override void OnNetworkSpawn()
    //{
    //    base.OnNetworkSpawn();
    //    if (IsLocalPlayer && (!leftController.isValid || !rightController.isValid || !headController.isValid))
    //    {
    //        InitInputDevices();
    //    }
    //}

    void Update()
    {
        if (IsLocalPlayer && (!leftController.isValid || !rightController.isValid || !headController.isValid))
        {
            InitInputDevices();
        }
    }

    private void InitInputDevices()
    {
        if (!leftController.isValid)
        {
            InitInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, ref leftController);
            leftController.subsystem.TryRecenter();
        }
        if (!rightController.isValid)
        {
            InitInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, ref rightController);
            rightController.subsystem.TryRecenter();
        }
        if (!headController.isValid)
        {
            InitInputDevice(InputDeviceCharacteristics.HeadMounted, ref headController);
            headController.subsystem.TryRecenter();
        }
    }

    private void InitInputDevice(InputDeviceCharacteristics inputDeviceCharacteristics, ref InputDevice inputDevice)
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(inputDeviceCharacteristics, devices);

        if (devices.Count > 0)
        {
            inputDevice = devices[0];
        }
    }
}

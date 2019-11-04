using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class GunInput : MonoBehaviour
{
    public SteamVR_Action_Boolean FireAction;
    public SteamVR_Input_Sources handType;
    public AbstractGun gun;

    // Update is called once per frame
    void Update()
    {

        if (FireAction.GetStateDown(handType))
        {
            Debug.Log("Fire Button Pressed");
            gun.Fire();

        }
    }
}

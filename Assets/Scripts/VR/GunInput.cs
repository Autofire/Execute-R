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

        if (FireAction.GetState(handType))
        {
            Debug.Log("Fire Button Pressed from controller");
            gun.Fire();

        }
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Fire Button Pressed from spacebar");
            gun.Fire();
        }
    }
}

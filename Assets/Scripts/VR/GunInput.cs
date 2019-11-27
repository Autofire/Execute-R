using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class GunInput : MonoBehaviour
{
    public SteamVR_Action_Boolean FireAction;
    public AbstractGun gun;

    private SteamVR_Input_Sources handType;

	private void Start() {
		handType = GetComponentInParent<SteamVR_Behaviour_Pose>().inputSource;
	}

    // Update is called once per frame
    void Update()
    {

        if (FireAction.GetStateDown(handType))
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

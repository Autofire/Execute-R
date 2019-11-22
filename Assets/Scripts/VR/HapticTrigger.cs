using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HapticTrigger : MonoBehaviour
{
    //public SteamVR_Input_Sources handType;
	public SteamVR_Action_Vibration vibration;
	public SteamVR_Input_Sources source;
	public float duration;
	[Range(0, 320)]
	public int frequency;
	[Range(0, 1)]
	public float amplitude;

	public void Pulse() {
		//SteamVR_Action_Vibration vibration;
		vibration.Execute(0, duration, frequency, amplitude, source);

	}
}

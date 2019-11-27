using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shotgun : AbstractGun
{
    public Transform barrelPoint;
	public Transform barrelPoint2;
	public GameObject bulletPrefab;
	public UnityEvent onFire;

    void Start()
    {
        //bulletSpeed = 4;
    }
    public override void Fire()
    {

        Debug.Log("Gun Fired");

        // Position of bullet on creation
        GameObject bullet = Instantiate(
			bulletPrefab,
			barrelPoint.transform.position,
			barrelPoint.rotation
		);
		GameObject bullet2 = Instantiate(
			bulletPrefab,
			barrelPoint2.transform.position,
			barrelPoint2.rotation
		);

		onFire.Invoke();

    }
}

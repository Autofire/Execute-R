using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DefaultGun : AbstractGun
{
    public Transform barrelPoint;
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

		onFire.Invoke();

    }
}

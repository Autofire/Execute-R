using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultGun : AbstractGun
{
    public Transform barrelPoint;
    public GameObject bulletPrefab;

    void Start()
    {
        //bulletSpeed = 4;
    }
    public override void Fire()
    {

        Debug.Log("Gun Fired");
        // Position of bullet on creation
        Vector3 start = barrelPoint.transform.position;
        //start += barrelPoint.transform.forward.normalized * 1;

        GameObject bullet = Instantiate(bulletPrefab, start, barrelPoint.rotation);
        
    }
}

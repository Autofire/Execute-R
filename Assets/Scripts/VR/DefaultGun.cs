using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultGun : AbstractGun
{
    public Transform barrelPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed;

    void Start()
    {
        //bulletSpeed = 4;
    }
    public override void Fire()
    {

        Debug.Log("Gun Fired");
        // Position of bullet on creation
        Vector3 start = barrelPoint.transform.position;
        start += barrelPoint.transform.forward.normalized * 1;

        GameObject bullet = Instantiate(bulletPrefab, start, barrelPoint.rotation);
        Rigidbody body = bullet.GetComponent<Rigidbody>();
        body.velocity = bullet.transform.forward * bulletSpeed;
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultGun : AbstractGun
{
    public Transform barrelPoint;
    public GameObject bulletPrefab;

    public override void Fire()
    {
        Debug.Log("Gun Fired");
        Instantiate(bulletPrefab, barrelPoint.position, barrelPoint.rotation);
        
    }
}

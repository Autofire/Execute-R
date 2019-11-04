using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultGun : AbstractGun
{
    public override void Fire()
    {
        Debug.Log("Gun Fired");
        
    }
}

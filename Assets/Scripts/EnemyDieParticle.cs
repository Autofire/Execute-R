using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDieParticle : MonoBehaviour
{
    private Vector3 pos;
    private Quaternion rot;

    public GameObject deathParticle;

    public void InstantiateParticles()
    {
        pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        rot = Quaternion.LookRotation(Vector3.up, Vector3.up);
        Instantiate(deathParticle, pos, rot);
    }
}

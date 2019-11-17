using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDieParticle : MonoBehaviour
{
    public GameObject deathParticle;

    public void InstantiateParticles()
    {
        Instantiate(deathParticle, gameObject.transform);
    }
}

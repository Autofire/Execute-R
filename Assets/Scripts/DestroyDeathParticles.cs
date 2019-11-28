using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDeathParticles : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(destroyParticles());
    }

    private IEnumerator destroyParticles()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}

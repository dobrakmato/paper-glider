using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireParticles : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            FireHitPlayer(other);
        }
    }

    private void FireHitPlayer(GameObject other)
    {
        other.GetComponent<AirplaneController>().Ignite();
    }
}
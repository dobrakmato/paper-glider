using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPlayer : MonoBehaviour
{
    private bool isPlayerDestroyed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isPlayerDestroyed)
            {
                isPlayerDestroyed = true;
                var apc = other.GetComponent<AirplaneController>();
                if (apc == null) apc = other.GetComponentInParent<AirplaneController>();
                apc.Explode();
                gameObject.GetComponentInParent<BrakingAiAirplane>().Explode();
            }
        }
    }
}
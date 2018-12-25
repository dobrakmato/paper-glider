using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameObject CoinPickup;

    private bool _isCoinPickedUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!_isCoinPickedUp)
            {
                _isCoinPickedUp = true;
                PickUpTheCoin(other.gameObject);
            }
        }
    }

    private void PickUpTheCoin(GameObject player)
    {
        var apc = player.GetComponent<AirplaneController>();
        if (apc == null) apc = player.GetComponentInParent<AirplaneController>();

        apc.GiveCoin();
        Instantiate(CoinPickup, transform.position, Quaternion.identity);
        PlayPickupSound(); // ~209ms
        StartCoroutine("AnimateThenDestroy"); // ~250ms
    }

    private void PlayPickupSound()
    {
        var audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
    }

    private IEnumerator AnimateThenDestroy()
    {
        const float steps = 15f; // quarter second
        var step = transform.localScale.x / steps;
        for (var i = 1; i <= steps; i++)
        {
            var f = step * (steps - i);
            transform.localScale = new Vector3(f, f, f);
            yield return new WaitForFixedUpdate();
        }

        Destroy(gameObject);
    }
}
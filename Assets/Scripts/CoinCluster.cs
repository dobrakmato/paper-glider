using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCluster : MonoBehaviour
{
    public GameObject Coin;

    private void Start()
    {
        var h = Random.Range(0f, 3f);
        transform.position = new Vector3(transform.position.x, h, transform.position.z);
        
        /* main coin */
        var main = Instantiate(Coin, transform);
        main.transform.localPosition = new Vector3();

        /* side coins */
        var sideCoins = Random.Range(5, 9);
        for (var i = 0; i < sideCoins; i++)
        {
            var side = Instantiate(Coin, transform);
            side.transform.localPosition = Random.onUnitSphere * 0.5f;
            side.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
    }
}
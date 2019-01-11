using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTrack : MonoBehaviour
{
    public void RandomizeLocation()
    {
        transform.localPosition =
            new Vector3(Random.Range(-1f, 2f), Random.Range(0f, 2f), transform.localPosition.z);
    }
}
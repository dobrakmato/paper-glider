using System.Collections;
using System.Collections.Generic;
using Level;
using UnityEngine;

public class CoinTrack : MonoBehaviour
{
    public GameObject UpperRow;

    public void RandomizeLocation()
    {
        transform.localPosition =
            new Vector3(LevelRandom.Range(-1.3f, 1.3f), LevelRandom.Range(0f, 2f), transform.localPosition.z);
    }

    public void DestroyUpperRow()
    {
        Destroy(UpperRow);
    }
}
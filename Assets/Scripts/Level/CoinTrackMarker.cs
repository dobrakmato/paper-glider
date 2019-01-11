using UnityEngine;

public class CoinTrackMarker : MonoBehaviour
{
    public Transform[] Markers;
    public GameObject CoinTrack;

    public void Generate()
    {
        var position = Markers[Random.Range(0, Markers.Length)];
        Instantiate(CoinTrack, position); // as parent of marker
    }
}
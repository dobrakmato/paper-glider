using Level;
using UnityEngine;

public class CoinTrackMarker : MonoBehaviour
{
    public Transform[] Markers;
    public GameObject CoinTrack;

    public void Generate()
    {
        var position = Markers[LevelRandom.Range(0, Markers.Length)];
        var track = Instantiate(CoinTrack, position); // as parent of marker
        track.GetComponent<CoinTrack>().DestroyUpperRow();
    }
}
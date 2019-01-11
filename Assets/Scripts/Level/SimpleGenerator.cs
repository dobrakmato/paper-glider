using System;
using UnityEngine;
using Random = UnityEngine.Random;

/* simplified version of very complicated generator */
public class SimpleGenerator : MonoBehaviour
{
    public GameObject[] EasyObstacles;
    public GameObject[] MediumObstacles;
    public GameObject[] HardObstacles;
    public GameObject[] CoinPrefabs;

    private enum Type
    {
        Obstacle,
        Coin
    }

    private Type _currentType = Type.Coin;
    private int _currentLeft = 2;

    private int _currentDifficulty = 1; // 1 = easy, 2 = medium, 3 = hard
    private int _currentPart = 1;

    public GameObject CreateNext(Vector3 at)
    {
        var obj = _currentType == Type.Obstacle ? CreateNextObstacle(at) : CreateNextCoin(at);

        DecreaseCurrentLeft();
        IncreaseCurrentPart();

        return obj;
    }

    private GameObject CreateNextObstacle(Vector3 at)
    {
        var obstacle = Instantiate(ChooseObstacle(), at, Quaternion.identity);

        var obstacleCoinTrackMarker = obstacle.GetComponent<CoinTrackMarker>();
        if (obstacleCoinTrackMarker != null) obstacleCoinTrackMarker.Generate();

        return obstacle;
    }

    private GameObject CreateNextCoin(Vector3 at)
    {
        var obj = Instantiate(ChooseCoin(), at, Quaternion.identity);

        var coinTrack = obj.GetComponent<CoinTrack>();
        if (coinTrack) coinTrack.RandomizeLocation();

        return obj;
    }

    private void DecreaseCurrentLeft()
    {
        _currentLeft--;
        if (_currentLeft == 0)
        {
            _currentType = _currentType == Type.Coin ? Type.Obstacle : Type.Coin;
            _currentLeft = Random.Range(1, 6);
        }
    }

    private GameObject ChooseCoin()
    {
        return CoinPrefabs[Random.Range(0, CoinPrefabs.Length)];
    }

    private void IncreaseCurrentPart()
    {
        _currentPart++;
        if (_currentPart > 30)
        {
            _currentDifficulty++;
            _currentPart = 0;
        }
    }

    private GameObject ChooseObstacle()
    {
        if (_currentDifficulty == 1)
        {
            return EasyObstacles[Random.Range(0, EasyObstacles.Length)];
        }

        if (_currentDifficulty == 2)
        {
            switch (Random.Range(0, 4))
            {
                case 0: return MediumObstacles[Random.Range(0, MediumObstacles.Length)];
                case 1: return MediumObstacles[Random.Range(0, MediumObstacles.Length)];
                case 2: return EasyObstacles[Random.Range(0, MediumObstacles.Length)];
                case 3: return EasyObstacles[Random.Range(0, EasyObstacles.Length)];
                default: throw new Exception("Should not happen");
            }
        }

        switch (Random.Range(0, 4))
        {
            case 0: return HardObstacles[Random.Range(0, HardObstacles.Length)];
            case 1: return HardObstacles[Random.Range(0, HardObstacles.Length)];
            case 2: return MediumObstacles[Random.Range(0, MediumObstacles.Length)];
            case 3: return EasyObstacles[Random.Range(0, EasyObstacles.Length)];
            default: throw new Exception("Should not happen");
        }
    }
}
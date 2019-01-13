using System;
using Level;
using UnityEngine;

/* simplified version of very complicated generator */
public class SimpleGenerator : MonoBehaviour
{
    public GameObject[] EasyObstacles;
    public GameObject[] MediumObstacles;
    public GameObject[] HardObstacles;
    public GameObject[] CoinPrefabs;
    public GameObject BrakingEnemy;

    private GameObject _player;

    private enum Type
    {
        Obstacle,
        Coin
    }

    private Type _currentType = Type.Coin;
    private int _currentLeft = 2;

    private int _currentDifficulty = 1; // 1 = easy, 2 = medium, 3 = hard
    private int _currentPart = 1;

    private void Start()
    {
        _player = FindObjectOfType<AirplaneController>().gameObject;
        Reset();
    }

    public GameObject CreateNext(Vector3 at)
    {
        var obj = _currentType == Type.Obstacle ? CreateNextObstacle(at) : CreateNextCoin(at);

        DecreaseCurrentLeft();
        IncreaseCurrentPart();

        return obj;
    }

    private GameObject CreateNextObstacle(Vector3 at)
    {
        if (_currentDifficulty > 1 && LevelRandom.Range(0f, 1f) > 0.9f)
        {
            var p = _player.transform.position;
            var targetY = p.y > 3f ? 1f : 4f;

            Instantiate(BrakingEnemy, new Vector3(-p.x, targetY, 9f), Quaternion.identity);
            return new GameObject();
        }

        var obstacle = Instantiate(ChooseObstacle(), at, Quaternion.identity);

        var obstacleCoinTrackMarker = obstacle.GetComponent<CoinTrackMarker>();
        if (obstacleCoinTrackMarker != null)
        {
            if (LevelRandom.Range(0f, 1f) > 0.2f) obstacleCoinTrackMarker.Generate();
        }

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
            if (_currentDifficulty >= 2 && _currentType == Type.Coin)
            {
                _currentLeft = LevelRandom.Range(1, 3);
            }
            _currentLeft = LevelRandom.Range(1, 6);
        }
    }

    private GameObject ChooseCoin()
    {
        return CoinPrefabs[LevelRandom.Range(0, CoinPrefabs.Length)];
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
            return EasyObstacles[LevelRandom.Range(0, EasyObstacles.Length)];
        }

        if (_currentDifficulty == 2)
        {
            switch (LevelRandom.Range(0, 4))
            {
                case 0: return MediumObstacles[LevelRandom.Range(0, MediumObstacles.Length)];
                case 1: return MediumObstacles[LevelRandom.Range(0, MediumObstacles.Length)];
                case 2: return EasyObstacles[LevelRandom.Range(0, EasyObstacles.Length)];
                case 3: return EasyObstacles[LevelRandom.Range(0, EasyObstacles.Length)];
                default: throw new Exception("Should not happen");
            }
        }

        switch (LevelRandom.Range(0, 4))
        {
            case 0: return HardObstacles[LevelRandom.Range(0, HardObstacles.Length)];
            case 1: return HardObstacles[LevelRandom.Range(0, HardObstacles.Length)];
            case 2: return MediumObstacles[LevelRandom.Range(0, MediumObstacles.Length)];
            case 3: return EasyObstacles[LevelRandom.Range(0, EasyObstacles.Length)];
            default: throw new Exception("Should not happen");
        }
    }

    public void Reset()
    {
        _currentType = Type.Coin;
        _currentLeft = 2;
        _currentDifficulty = 1;
        _currentPart = 1;
    }
}
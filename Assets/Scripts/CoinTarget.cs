using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinTarget : MonoBehaviour
{
    public GameObject CoinPrefab;

    private Vector3 _center;
    private const int Levels = 2;
    private readonly int[] _levelCounts = new int[Levels] {8, 12};
    private readonly float[] _levelSizes = new float[Levels] {0.6f, 0.4f};
    private readonly float[] _levelDistances = new float[Levels] {1.9f, 3.4f};

    void Start()
    {
        var sizeHalf = GetComponent<BoxCollider>().size / 2;
        var center = GetComponent<BoxCollider>().center;
        _center = new Vector3(
            Random.Range(center.x - sizeHalf.x, center.x + sizeHalf.x),
            Random.Range(center.y - sizeHalf.y, center.y + sizeHalf.y) + 1.5f, // level generator is retarded
            Random.Range(center.z - sizeHalf.z, center.z + sizeHalf.z)
        );
        
        SpawnCoinPrefabs();
    }

    private void SpawnCoinPrefabs()
    {
        /* main level */
        SpawnCoinPrefab(_center, 0.8f);

        /* small levels */
        for (int level = 0; level < Levels; level++)
        {
            var levelCunt = _levelCounts[level];
            for (int i = 0; i < levelCunt; i++)
            {
                var radian = (i + 1) * (2 * Mathf.PI / levelCunt);
                var offset = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * _levelDistances[level];

                SpawnCoinPrefab(_center + offset, _levelSizes[level]);
            }
        }
    }

    private void SpawnCoinPrefab(Vector3 localPosition, float size)
    {
        var obj = Instantiate(CoinPrefab, transform);
        obj.transform.localPosition = localPosition;
        obj.transform.localScale = new Vector3(size, size, size);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var maxDist = 5f;
            var dist = Vector3.Distance(other.transform.position, _center);
            var pct = (maxDist - dist) / maxDist;


            Debug.Log(pct);
        }
    }
}
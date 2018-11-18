using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] Objects;
    public float Speed = 1f;
    public float GenerateZ = -1f;
    public bool IsDead;

    private float Z_TO_REMOVE = 5f;
    private readonly List<GameObject> _visibleObjects = new List<GameObject>(256);

    void Start()
    {
        StartGeneration();
    }

    private void StartGeneration()
    {
        if (!IsDead)
        {
            GenerateSection();
        }

        Invoke("StartGeneration", 2f);
    }

    void Update()
    {
        MoveVisibleObjects();
        RemoveInvisibleObjects();
    }

    private void GenerateSection()
    {
        var obj = Instantiate(
            Objects[Random.Range(0, Objects.Length)],
            new Vector3(0, 0, GenerateZ),
            Quaternion.identity
        );
        _visibleObjects.Add(obj);
    }

    private void MoveVisibleObjects()
    {
        for (var i = 0; i < _visibleObjects.Count; i++)
        {
            var obj = _visibleObjects[i];
            obj.transform.position = obj.transform.position - new Vector3(0, 0, -Speed * Time.deltaTime);
        }
    }

    private void RemoveInvisibleObjects()
    {
        for (var i = 0; i < _visibleObjects.Count; i++)
        {
            if (_visibleObjects[i].transform.position.z > Z_TO_REMOVE)
            {
                var obj = _visibleObjects[i];
                _visibleObjects.RemoveAt(i);
                Destroy(obj);
            }
        }
    }
}
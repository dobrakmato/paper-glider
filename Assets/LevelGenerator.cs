using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] Objects;
    public float Speed = 1f;
    public float GenerateZ = -1f;
    public bool IsDead;

    private float Z_TO_REMOVE = 25f;
    private readonly List<GameObject> _visibleObjects = new List<GameObject>(256);

    public GameObject EnvTerrain;
    private int envChunkZId;
    private float envGenerateZ = -128f;
    private float envChunkZSize = 40f;
    private float envChunkZScale = 2f;
    private float envRemaining;
    private GameObject envLastGeneratedTerrain;
    
    void Start()
    {
        StartGeneration();

        envChunkZScale = EnvTerrain.transform.localScale.z;
        GenerateInitialEnvironment();
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

    private void TryGenerateEnv()
    {
        envRemaining += -Speed * Time.deltaTime;
      
        if (envRemaining < 0)
        {
            GenerateEnv();
        }
    }

    private void GenerateInitialEnvironment()
    {
        var s = 0f;
        while (s > envGenerateZ)
        {
            var obj = Instantiate(
                EnvTerrain,
                new Vector3(0, -1.5f, s),
                Quaternion.identity
            );
            var pgt = obj.GetComponent<ProceduralGeneratedTerrain>();
            pgt.GenerateAt = new Vector3(0, -1, -(envChunkZId++ * envChunkZSize / envChunkZScale));
            pgt.GenerateDuplicated();
            envLastGeneratedTerrain = obj;
            s -= envChunkZSize;
            _visibleObjects.Add(obj);
        }
    }

    private void FixedUpdate()
    {
        TryGenerateEnv();
    }

    private void GenerateEnv()
    {
        envRemaining = envChunkZSize;
        var genZ = envLastGeneratedTerrain != null ? envLastGeneratedTerrain.transform.position.z - envChunkZSize : envGenerateZ; 
        var obj = Instantiate(
            EnvTerrain,
            new Vector3(0, -1.5f, genZ),
            Quaternion.identity
        );
        var pgt = obj.GetComponent<ProceduralGeneratedTerrain>();
        pgt.GenerateAt = new Vector3(0, -1, -(envChunkZId++ * envChunkZSize / envChunkZScale));
        pgt.GenerateDuplicated();
        envLastGeneratedTerrain = obj;
        _visibleObjects.Add(obj);
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
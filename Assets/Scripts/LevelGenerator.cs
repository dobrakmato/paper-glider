using System.Collections;
using System.Collections.Generic;
using Level;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    /* Default player speed. */
    public const float DefaultSpeed = 32f; // 16f

    public float Speed = 1f;
    public float GenerateZ = -1f;
    public bool IsDead;
    public bool IsGenerating = true;

    private float Z_TO_REMOVE = 25f;
    private readonly List<GameObject> _visibleObjects = new List<GameObject>(256);

    public AudioSource SpeedUpSoundSource;

    public Material Skybox;

    public GameObject EnvTerrain;
    private int envChunkZId;
    private float envGenerateZ = -128f;
    private float envChunkZSize = 40f;
    private float envChunkZScale = 2f;
    private GameObject envLastGeneratedTerrain;

    private SimpleGenerator _generator;

    void Start()
    {
        _generator = GetComponent<SimpleGenerator>();
        StartGeneration();
        GenerateSkybox();

        envChunkZScale = EnvTerrain.transform.localScale.z;
        GenerateInitialEnvironment();
    }

    private void GenerateSkybox()
    {
        Skybox.SetFloat("_SkyExponent1", Random.Range(1.5f, 9f));
    }

    private void StartGeneration()
    {
        if (!IsDead && IsGenerating)
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
        /* for first environment part variable is null */
        if (envLastGeneratedTerrain == null || envLastGeneratedTerrain.transform.position.z > GenerateZ)
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
        var genZ = envLastGeneratedTerrain != null
            ? envLastGeneratedTerrain.transform.position.z - envChunkZSize
            : envGenerateZ;
        
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
        var obj = _generator.CreateNext(new Vector3(0, 0, GenerateZ));
        _visibleObjects.Add(obj);
    }

    private void MoveVisibleObjects()
    {
        for (var i = 0; i < _visibleObjects.Count; i++)
        {
            var obj = _visibleObjects[i];
            if (obj != null)
            {
                obj.transform.position = obj.transform.position - new Vector3(0, 0, -Speed * Time.deltaTime);
            }
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

    public struct SpeedUpParams
    {
        public float TargetSpeed { get; set; }
        public float Time { get; set; }
        public float TargetFov { get; set; }
        public bool PlaySound { get; set; }
    }

    public void RestartLevel()
    {
        IsDead = false;
        IsGenerating = true;
        LevelRandom.ResetBySeeder();
        _generator.Reset();
        FindObjectOfType<Skins>().SetLastBoughtSkin();

        StartCoroutine("LevelSpeedup", new SpeedUpParams
        {
            Time = 0.5f,
            TargetSpeed = DefaultSpeed,
            TargetFov = 70f
        });
    }

    public IEnumerator LevelSpeedup(SpeedUpParams p)
    {
        if (p.PlaySound)
        {
            SpeedUpSoundSource.Play();
        }

        var elapsed = 0f;
        var old = Speed;
        var oldFov = Camera.main.fieldOfView;
        while (elapsed < p.Time)
        {
            var f = elapsed / p.Time;
            Speed = Mathf.Lerp(old, p.TargetSpeed, f);
            Camera.main.fieldOfView = Mathf.Lerp(oldFov, p.TargetFov, f);
            elapsed += Time.deltaTime;
            yield return null; // next frame
        }
    }
}
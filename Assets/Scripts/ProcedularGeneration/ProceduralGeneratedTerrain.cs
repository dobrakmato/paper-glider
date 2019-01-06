using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralGeneratedTerrain : MonoBehaviour
{
    public int Subdivisons = 10;
    public float BaseSize = 10f; // from unity plane
    public Vector3 GenerateAt;

    public GameObject EnvTree;

    [NonSerialized] public int Seed = 0;

    private Mesh _mesh;

    private float OffsetPerlinNoise(float x, float y)
    {
        var seedOffsetX = 47 + Seed * 7 % 10000;
        var seedOffsetY = 83 + Seed * 11 % 10000;

        return Mathf.PerlinNoise(seedOffsetX + x, seedOffsetY + y);
    }

    private float Noise(float x, float y)
    {
        var amp2 = OffsetPerlinNoise(y * 0.005f, 6947f) * 2f;
        var r = Mathf.Clamp(OffsetPerlinNoise(y * 0.005f, 47511f), 0.1f, 1f);
        var r2 = Mathf.Clamp(OffsetPerlinNoise(y * 0.0001f, 13f), 0.2f, 1f);

        var shape = Mathf.Clamp(0.05f * x * x, 0, 3f);
        var o1 = OffsetPerlinNoise(x * r2, y * r2) * 10f * r;
        var o2 = OffsetPerlinNoise(x * 0.25f, y * 0.25f) * 4f * r * r;
        var o3 = OffsetPerlinNoise(x * 0.1f, y * 0.1f) * 2f * r * r * r;

        var amp = OffsetPerlinNoise(y * 0.1f, 37589f) * 32f;

        var res = (o1 + o2 + o3) / 16f * shape * amp * amp2;

        if (Mathf.Abs(x) > 4f && r <= 0.4f)
        {
            if (Random.Range(0f, 1f) > 0.6 + r)
            {
                if (Random.Range(0f, 1f) > 0.9f)
                {
                    Instantiate(EnvTree, new Vector3(2 * x, res - 1.5f, y / 2), Quaternion.identity, transform);
                }
            }
        }

        return res;
    }

    private Vector3 F(Vector3 min, int i, Vector3 size, int j)
    {
        var absx = min.x + (float) i / Subdivisons * size.x;
        var absz = min.z + (float) j / Subdivisons * size.z;
        var n = Noise(absx, absz);
        return new Vector3(absx - GenerateAt.x, n, absz - GenerateAt.z);
    }

    public void GenerateDuplicated()
    {
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _mesh.name = "Procedural Terrain";

        var size = transform.localScale * BaseSize;
        var min = GenerateAt - size / 2;
        var vertices = new Vector3[Subdivisons * Subdivisons * 3 * 2];
        var colors = new Color[Subdivisons * Subdivisons * 3 * 2];
        var triangles = new int[Subdivisons * Subdivisons * 3 * 2];
        var k = 0;
        for (var i = 0; i < Subdivisons; i++)
        {
            for (var j = 0; j < Subdivisons; j++)
            {
                var n = F(min, i, size, j);
                var n1 = F(min, i, size, j + 1);
                var nn = F(min, i + 1, size, j);
                var nn1 = F(min, i + 1, size, j + 1);


                var c0 = new Color(0, 0.6f, n.y / 20f);
                triangles[k] = k;
                vertices[k++] = n;
                triangles[k] = k;
                vertices[k++] = n1;
                triangles[k] = k;
                vertices[k++] = nn;
                colors[k - 1] = c0;
                colors[k - 2] = c0;
                colors[k - 3] = c0;

                var c1 = new Color(0, 0.6f, n.y / 20f);
                triangles[k] = k;
                vertices[k++] = n1;
                triangles[k] = k;
                vertices[k++] = nn1;
                triangles[k] = k;
                vertices[k++] = nn;
                colors[k - 1] = c1;
                colors[k - 2] = c1;
                colors[k - 3] = c1;
            }
        }

        _mesh.vertices = vertices;
        _mesh.colors = colors;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
    }

    public void GenerateIndexed()
    {
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _mesh.name = "Procedural Terrain";

        var size = transform.localScale * BaseSize;
        var min = GenerateAt - size / 2;
        var verticesInRow = Subdivisons + 1;
        var vertices = new Vector3[verticesInRow * verticesInRow];
        var colors = new Color[verticesInRow * verticesInRow];
        for (var i = 0; i <= Subdivisons; i++)
        {
            for (var j = 0; j <= Subdivisons; j++)
            {
                var absx = min.x + (float) i / Subdivisons * size.x;
                var absz = min.z + (float) j / Subdivisons * size.z;
                var n = Noise(absx, absz);
                vertices[i * verticesInRow + j] = new Vector3(absx - GenerateAt.x, n, absz - GenerateAt.z);


                colors[i * verticesInRow + j] = new Color(0, 0.6f, n / 20f);
            }
        }

        _mesh.vertices = vertices;
        _mesh.colors = colors;


        var triangles = new int[Subdivisons * Subdivisons * 3 * 2];
        var k = 0;
        for (var i = 0; i < Subdivisons; i++)
        {
            for (var j = 0; j < Subdivisons; j++)
            {
                var idx = i * verticesInRow + j;

                triangles[k++] = idx;
                triangles[k++] = idx + 1;
                triangles[k++] = idx + verticesInRow;

                triangles[k++] = idx + 1;
                triangles[k++] = idx + 1 + verticesInRow;
                triangles[k++] = idx + verticesInRow;
            }
        }

        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
    }
}
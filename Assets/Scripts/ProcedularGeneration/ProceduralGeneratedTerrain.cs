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

    private Mesh _mesh;

    private float Noise(float x, float y)
    {
        var amp2 = Mathf.PerlinNoise(y * 0.005f, 6947f) * 2f;
        var r = Mathf.Clamp(Mathf.PerlinNoise(y * 0.005f, 47511f), 0.2f, 1f);
        
        var shape = Mathf.Clamp(0.05f * x * x, 0, 3f);
        var o1 = Mathf.PerlinNoise(x * 1f, y * 1f) * 10f;
        var o2 = Mathf.PerlinNoise(x * 0.25f, y * 0.25f) * 4f * r;
        var o3 = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) * 2f * r;

        var amp = Mathf.PerlinNoise(y * 0.1f, 37589f) * 32f;
        
        return (o1 + o2 + o3) / 16f * shape * amp * amp2;
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
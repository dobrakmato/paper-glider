using System;
using System.Collections;
using Level;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralGeneratedTerrain : MonoBehaviour
{
    public int Subdivisons = 10;
    public int TreeSegments = 20;
    public float TreeMultiplier = 1.3f;
    public float BaseSize = 10f; // from unity plane
    public Vector3 GenerateAt;

    public GameObject EnvTree;

    [NonSerialized] public int Seed = Seeder.ComputeSeed();

    private Mesh _mesh;

    /* map for seeding tree */
    private readonly ulong[] treeSeeded = new ulong[64]; // 64 * 64 flags

    private bool CanPlantTree(int segX, int segY)
    {
        var mask = (ulong) (1 << segX);
        return (treeSeeded[segY] & mask) != mask;
    }

    private void MarkTreePlanted(int segX, int segY)
    {
        var mask = (ulong) (1 << segX);
        treeSeeded[segY] |= mask;
    }

    private float OffsetPerlinNoise(float x, float y)
    {
        var seedOffsetX = 47 + Seed * 7 % 10000;
        var seedOffsetY = 83 + Seed * 11 % 10000;

        return Mathf.PerlinNoise(seedOffsetX + x, seedOffsetY + y);
    }

    private float Noise(float x, float y)
    {
        var shapeX = Mathf.Clamp(0.05f * x * x, 0, 3f); // shape of terrain based on x (U-curve)

        var amp = OffsetPerlinNoise(x * 0.01f, y * 0.01f) * 6f;
        var mix = OffsetPerlinNoise(48 + x * 0.01f, 19 + y * 0.01f);

        var o1 = OffsetPerlinNoise(x * 0.7f, y * 0.7f) * amp * amp;
        var o2 = OffsetPerlinNoise(x * 0.3f, y * 0.3f) * amp * amp;

        var res = Mathf.Lerp(o1, o2, mix) * shapeX;

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

        var totalZDiff = 0f;
        var size = transform.localScale * BaseSize;
        var halfSize = size * 0.5f;
        var min = GenerateAt - size / 2;
        var vertices = new Vector3[Subdivisons * Subdivisons * 3 * 2];
        var colors = new Color[Subdivisons * Subdivisons * 3 * 2];
        var triangles = new int[Subdivisons * Subdivisons * 3 * 2];
        var k = 0;
        for (var i = 0; i < Subdivisons; i++)
        {
            for (var j = 0; j < Subdivisons; j++)
            {
                /* todo: optimize to call F(a,b) only once for different a,b */
                var n = F(min, i, size, j);
                var n1 = F(min, i, size, j + 1);
                var nn = F(min, i + 1, size, j);
                var nn1 = F(min, i + 1, size, j + 1);

                totalZDiff += Math.Abs(n.y- n1.y);

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


        var segmentSize = size.x / TreeSegments;

        for (var i = 0; i < vertices.Length; i++)
        {
            var vert = vertices[i];
            
            if (Mathf.Abs(vert.x) < 4.5f) continue;
            if (Mathf.Abs(vert.y) > 12f) continue;
            if (Random.Range(0f, 1f) < totalZDiff/1000) continue; /* this random is not gameplay-related */

            var segX = (int) map(vert.x, -10f, 10f, 0, TreeSegments);
            var segZ = (int) map(vert.z, -10f, 10f, 0, TreeSegments);

            if (CanPlantTree(segX, segZ))
            {
                MarkTreePlanted(segX, segZ);
                var obj = Instantiate(EnvTree, new Vector3(), Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
                obj.transform.localPosition = new Vector3(
                    Random.Range(0, segmentSize) - halfSize.x + segX * size.x / TreeSegments,
                    vert.y + Random.Range(-2f, 0),
                    Random.Range(0, segmentSize) - halfSize.z + segZ * size.z / TreeSegments
                );
            }
        }

        _mesh.vertices = vertices;
        _mesh.colors = colors;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
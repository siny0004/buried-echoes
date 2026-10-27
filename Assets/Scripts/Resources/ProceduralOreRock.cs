
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralOreRock : MonoBehaviour
{
    [Header("Rock Shape")]
    [SerializeField] private int seed = 123;

    [SerializeField, Range(0f, 0.5f)]
    private float irregularity = 0.25f;

    [SerializeField]
    private Vector3 dimensions =
        new Vector3(0.22f, 0.18f, 0.20f);

    [SerializeField, Range(0, 3)]
    private int subdivisions = 2;

    [Header("Surface Smoothing")]
    [SerializeField, Range(0f, 1f)]
    private float smoothing = 0.25f;

    private Mesh generatedMesh;

    private void Awake()
    {
        GenerateRock();
    }

    public void GenerateRock()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Create the initial icosahedron.
        CreateIcosahedron(vertices, triangles);

        // Subdivide each triangle into four smaller triangles.
        for (int i = 0; i < subdivisions; i++)
        {
            Subdivide(vertices, triangles);
        }

        // Distort vertices to create an irregular rock shape.
        System.Random random = new System.Random(seed);

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 direction = vertices[i].normalized;

            // Broad surface variation.
            float noise = Mathf.PerlinNoise(
                direction.x * 3f + seed * 0.01f,
                direction.y * 3f + direction.z * 2f
            );

            float broadVariation =
                (noise - 0.5f) * 2f * irregularity;

            // Smaller random irregularities.
            float detailVariation =
                ((float)random.NextDouble() * 2f - 1f)
                * irregularity * 0.25f;

            float radius = Mathf.Max(
                0.3f,
                1f + broadVariation + detailVariation
            );

            vertices[i] = Vector3.Scale(
                direction * radius,
                dimensions
            );
        }

        // Duplicate triangle vertices to produce flat shading.
        Vector3[] flatVertices = new Vector3[triangles.Count];
        int[] flatTriangles = new int[triangles.Count];

        for (int i = 0; i < triangles.Count; i++)
        {
            flatVertices[i] = vertices[triangles[i]];
            flatTriangles[i] = i;
        }

        Mesh newMesh = new Mesh();
        newMesh.name = "Procedural Ore Rock";
        newMesh.vertices = flatVertices;
        newMesh.triangles = flatTriangles;

        // Calculate the flat face normals first.
        newMesh.RecalculateNormals();

        Vector3[] flatNormals = newMesh.normals;

        // Calculate smooth normals by averaging adjacent faces
        // that share the same original vertex.
        Vector3[] smoothNormals = new Vector3[flatVertices.Length];
        Vector3[] normalSums = new Vector3[vertices.Count];

        for (int i = 0; i < triangles.Count; i++)
        {
            normalSums[triangles[i]] += flatNormals[i];
        }

        for (int i = 0; i < triangles.Count; i++)
        {
            smoothNormals[i] = normalSums[triangles[i]].normalized;
        }

        // Blend between flat and smooth shading.
        Vector3[] blendedNormals = new Vector3[flatVertices.Length];

        for (int i = 0; i < blendedNormals.Length; i++)
        {
            blendedNormals[i] = Vector3.Lerp(
                flatNormals[i],
                smoothNormals[i],
                smoothing
            ).normalized;
        }

        newMesh.normals = blendedNormals;
        newMesh.RecalculateBounds();


        GetComponent<MeshFilter>().sharedMesh = newMesh;

        if (generatedMesh != null)
        {
            if (Application.isPlaying)
                Destroy(generatedMesh);
            else
                DestroyImmediate(generatedMesh);
        }

        generatedMesh = newMesh;
    }

    public void RandomizeShape(int newSeed, float sizeVariation)
    {
        seed = newSeed;

        // Randomize the dimensions independently on each axis.
        System.Random random = new System.Random(newSeed);

        Vector3 baseDimensions = new Vector3(
            0.22f, 0.17f, 0.20f
        );

        dimensions = new Vector3(
            baseDimensions.x * RandomScale(random, sizeVariation),
            baseDimensions.y * RandomScale(random, sizeVariation),
            baseDimensions.z * RandomScale(random, sizeVariation)
        );

        GenerateRock();
    }

    private float RandomScale(System.Random random, float variation)
    {
        return 1f +
            ((float)random.NextDouble() * 2f - 1f) * variation;
    }


    private void CreateIcosahedron(
        List<Vector3> vertices,
        List<int> triangles)
    {
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        Vector3[] points =
        {
            new(-1, t, 0), new(1, t, 0),
            new(-1, -t, 0), new(1, -t, 0),
            new(0, -1, t), new(0, 1, t),
            new(0, -1, -t), new(0, 1, -t),
            new(t, 0, -1), new(t, 0, 1),
            new(-t, 0, -1), new(-t, 0, 1)
        };

        foreach (Vector3 point in points)
            vertices.Add(point.normalized);

        int[] faces =
        {
            0,11,5, 0,5,1, 0,1,7, 0,7,10, 0,10,11,
            1,5,9, 5,11,4, 11,10,2, 10,7,6, 7,1,8,
            3,9,4, 3,4,2, 3,2,6, 3,6,8, 3,8,9,
            4,9,5, 2,4,11, 6,2,10, 8,6,7, 9,8,1
        };

        triangles.AddRange(faces);
    }

    private void Subdivide(
        List<Vector3> vertices,
        List<int> triangles)
    {
        List<int> newTriangles = new List<int>();
        Dictionary<long, int> midpointCache =
            new Dictionary<long, int>();

        int GetMidpoint(int a, int b)
        {
            int min = Mathf.Min(a, b);
            int max = Mathf.Max(a, b);

            long key = ((long)min << 32) | (uint)max;

            if (midpointCache.TryGetValue(key, out int index))
                return index;

            Vector3 midpoint =
                (vertices[a] + vertices[b]).normalized;

            index = vertices.Count;
            vertices.Add(midpoint);
            midpointCache[key] = index;

            return index;
        }

        for (int i = 0; i < triangles.Count; i += 3)
        {
            int a = triangles[i];
            int b = triangles[i + 1];
            int c = triangles[i + 2];

            int ab = GetMidpoint(a, b);
            int bc = GetMidpoint(b, c);
            int ca = GetMidpoint(c, a);

            newTriangles.AddRange(new[]
            {
                a, ab, ca,
                b, bc, ab,
                c, ca, bc,
                ab, bc, ca
            });
        }

        triangles.Clear();
        triangles.AddRange(newTriangles);
    }

    private void OnDestroy()
    {
        if (generatedMesh == null)
            return;

        if (Application.isPlaying)
            Destroy(generatedMesh);
        else
            DestroyImmediate(generatedMesh);
    }
}

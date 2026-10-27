
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DrillSpiral : MonoBehaviour
{
    [Header("Spiral Shape")]
    [SerializeField, Min(0.01f)] private float length = 0.36f;
    [SerializeField, Min(0.01f)] private float innerRadius = 0.03f;
    [SerializeField, Min(0.01f)] private float outerRadius = 0.09f;
    [SerializeField, Min(1)] private int turns = 4;
    [SerializeField, Range(8, 128)] private int segments = 64;

    [Header("Blade Thickness")]
    [SerializeField, Min(0.001f)] private float thickness = 0.012f;

    private Mesh generatedMesh;

    private void Start()
    {
        GenerateSpiral();
    }

    private void OnDestroy()
    {
        if (generatedMesh != null)
            Destroy(generatedMesh);
    }

    private void GenerateSpiral()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Generated Drill Spiral";

        Vector3[] vertices = new Vector3[(segments + 1) * 4];
        int[] triangles = new int[segments * 24];

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;

            float angle = t * turns * Mathf.PI * 2f;
            float z = (t - 0.5f) * length;

            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            int index = i * 4;

            vertices[index] =
                new Vector3(
                    cos * innerRadius,
                    sin * innerRadius,
                    z
                );

            vertices[index + 1] =
                new Vector3(
                    cos * outerRadius,
                    sin * outerRadius,
                    z
                );

            vertices[index + 2] =
                new Vector3(
                    cos * innerRadius,
                    sin * innerRadius,
                    z + thickness
                );

            vertices[index + 3] =
                new Vector3(
                    cos * outerRadius,
                    sin * outerRadius,
                    z + thickness
                );
        }

        int triangleIndex = 0;

        for (int i = 0; i < segments; i++)
        {
            int a = i * 4;
            int b = (i + 1) * 4;

            // Front face
            AddQuad(triangles, ref triangleIndex,
                a, a + 1, b + 1, b);

            // Back face
            AddQuad(triangles, ref triangleIndex,
                a + 2, b + 2, b + 3, a + 3);

            // Outer edge
            AddQuad(triangles, ref triangleIndex,
                a + 1, a + 3, b + 3, b + 1);

            // Inner edge
            AddQuad(triangles, ref triangleIndex,
                a, b, b + 2, a + 2);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        generatedMesh = mesh;
        GetComponent<MeshFilter>().sharedMesh = generatedMesh;
    }

    private void AddQuad(
        int[] triangles,
        ref int index,
        int a, int b, int c, int d)
    {
        triangles[index++] = a;
        triangles[index++] = b;
        triangles[index++] = c;

        triangles[index++] = a;
        triangles[index++] = c;
        triangles[index++] = d;
    }
}

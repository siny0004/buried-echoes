using UnityEngine;
using UnityEditor;
using System.IO;

public static class ShedGableGenerator
{
    [MenuItem("Tools/Buried Echoes/Create Shed Gable")]
    public static void CreateGable()
    {
        // Gable dimensions
        float halfWidth = 6.65f;
        float bottomY = 8.9f;
        float peakY = 11.32f;
        float thickness = 0.2f;

        float frontZ = thickness / 2f;
        float backZ = -thickness / 2f;

        Vector3[] vertices =
        {
            // Front triangle
            new(-halfWidth, bottomY, frontZ),
            new(halfWidth, bottomY, frontZ),
            new(0, peakY, frontZ),

            // Back triangle
            new(-halfWidth, bottomY, backZ),
            new(halfWidth, bottomY, backZ),
            new(0, peakY, backZ)
        };

        int[] triangles =
        {
            // Front
            0, 1, 2,

            // Back
            5, 4, 3,

            // Bottom
            0, 3, 4,
            0, 4, 1,

            // Left slope
            0, 2, 5,
            0, 5, 3,

            // Right slope
            1, 4, 5,
            1, 5, 2
        };

        Mesh mesh = new Mesh();
        mesh.name = "ShedGableMesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Basic UV mapping for the front and rear.
        mesh.uv = new Vector2[]
        {
            new(0, 0), new(1, 0), new(0.5f, 1),
            new(0, 0), new(1, 0), new(0.5f, 1)
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        string folder = "Assets/GeneratedMeshes";

        if (!AssetDatabase.IsValidFolder(folder))
            Directory.CreateDirectory(folder);

        string path = folder + "/ShedGableMesh.asset";

        // Avoid replacing a previously generated mesh asset.
        path = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();

        GameObject wall = new GameObject("Wall_BackUpper");

        GameObject shed = GameObject.Find("MiningShed");

        if (shed != null)
            wall.transform.SetParent(shed.transform);

        // Position in world coordinates.
        wall.transform.position = new Vector3(0, 0, -40.5f);

        MeshFilter filter = wall.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh;

        wall.AddComponent<MeshRenderer>();

        Selection.activeGameObject = wall;

        Debug.Log("Shed gable created successfully!");
    }
}
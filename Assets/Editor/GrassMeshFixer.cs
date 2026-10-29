
using UnityEngine;
using UnityEditor;

public static class GrassMeshFixer
{
    [MenuItem("Tools/Buried Echoes/Fix Grass Mesh Orientation")]
    public static void FixGrassMesh()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            Debug.LogWarning("Select a grass GameObject first.");
            return;
        }

        MeshFilter meshFilter = selected.GetComponent<MeshFilter>();

        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogWarning("Selected object has no Mesh Filter or mesh.");
            return;
        }

        Mesh original = meshFilter.sharedMesh;

        // Create an independent copy of the imported mesh.
        Mesh corrected = Object.Instantiate(original);
        corrected.name = original.name + "_Upright";

        Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);

        // Rotate actual vertex positions.
        Vector3[] vertices = corrected.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = rotation * vertices[i];
        }

        corrected.vertices = vertices;

        // Rotate normals to preserve lighting.
        Vector3[] normals = corrected.normals;

        if (normals.Length == vertices.Length)
        {
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = rotation * normals[i];
            }

            corrected.normals = normals;
        }

        // Rotate tangents if the mesh has them.
        Vector4[] tangents = corrected.tangents;

        if (tangents.Length == vertices.Length)
        {
            for (int i = 0; i < tangents.Length; i++)
            {
                Vector3 tangent = rotation * new Vector3(
                    tangents[i].x,
                    tangents[i].y,
                    tangents[i].z
                );

                tangents[i] = new Vector4(
                    tangent.x,
                    tangent.y,
                    tangent.z,
                    tangents[i].w
                );
            }

            corrected.tangents = tangents;
        }

        corrected.RecalculateBounds();

        string path = AssetDatabase.GenerateUniqueAssetPath(
            "Assets/GrassMesh_Upright.asset"
        );

        AssetDatabase.CreateAsset(corrected, path);
        AssetDatabase.SaveAssets();

        meshFilter.sharedMesh = corrected;

        EditorUtility.SetDirty(selected);

        Debug.Log("Corrected grass mesh saved to: " + path);
    }
}

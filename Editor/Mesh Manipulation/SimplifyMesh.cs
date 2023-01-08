using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SimplifyMesh
{
    #region Simplify methods
    public static Mesh Simplify(Mesh inputMesh, ClusterNode[] clusterRoots)
    {
        Logger.LogInfo($"Saving simplification process of mesh {inputMesh.name}");
        ResizableArray<Vector3> vects = new ResizableArray<Vector3>(inputMesh.vertices);
        ResizableArray<Vector3> normals = new ResizableArray<Vector3>(inputMesh.normals);
        ResizableArray<Vector2> uvs = new ResizableArray<Vector2>(inputMesh.uv);
        List<Triangle> triangels = Triangles.TransformIntToVec3(inputMesh.triangles);
        List<int> indicesToDelete = new List<int>();

        foreach (ClusterNode root in clusterRoots)
        {
            CalculateMeshByClusterTree(root, ref vects, ref normals, ref uvs, ref triangels, ref indicesToDelete);
        }

        CleanMeshData(ref vects, ref normals, ref uvs, ref triangels, indicesToDelete.ToArray());

        Mesh resultMesh = new Mesh();
        resultMesh.vertices = vects.Data;
        resultMesh.normals = normals.Data;
        resultMesh.uv = uvs.Data;
        resultMesh.triangles = Triangles.TransformVec3ToInt(triangels);

        resultMesh.RecalculateBounds();
        resultMesh.RecalculateNormals();
        resultMesh.RecalculateTangents();

        Logger.LogInfo($"Finished saving simplification process of mesh {inputMesh.name}");
        return resultMesh;
    }

    private static void CleanMeshData(ref ResizableArray<Vector3> verts, ref ResizableArray<Vector3> normals, ref ResizableArray<Vector2> uvs, ref List<Triangle> triangels, int[] indicesToDelete)
    {
        // Sort indices to delete
        Array.Sort(indicesToDelete);
        Array.Reverse(indicesToDelete);

        foreach (int index in indicesToDelete)
        {
            verts.RemoveAt(index);
            normals.RemoveAt(index);
            uvs.RemoveAt(index);

            if (index < verts.Length)
            {
                ClearTriangles(ref triangels, index);
            }
        }
    }

    private static void ClearTriangles(ref List<Triangle> triangles, int index)
    {
        foreach (Triangle triangle in triangles)
        {
            for (int i = 0; i < triangle.data.Length; i++)
            {
                if (triangle.data[i] > index) { triangle.data[i]--; }
            }
        }
    }

    private static void CalculateMeshByClusterTree(ClusterNode head, ref ResizableArray<Vector3> verts, ref ResizableArray<Vector3> normals, ref ResizableArray<Vector2> uvs, ref List<Triangle> triangels, ref List<int> indicesToDelete)
    {
        // Not leaf
        if (head.children != null)
        {
            Dictionary<Triangle, int> toChangeTriangles = new Dictionary<Triangle, int>();
            Dictionary<Triangle, bool> toDeleteTriangles = new Dictionary<Triangle, bool>();

            // Iterate through all children
            foreach (ClusterNode cluster in head.children)
            {
                // Calculate data of child
                CalculateMeshByClusterTree(cluster, ref verts, ref normals, ref uvs, ref triangels, ref indicesToDelete);

                // Mark changes in triangles
                foreach (Triangle triangle in triangels)
                {
                    // Skip deleted
                    if (toDeleteTriangles.ContainsKey(triangle)) { continue; }

                    // Check triangle have this index
                    int foundIndex = triangle.Contains(cluster.index);

                    // Delete if multiple occurs
                    if (foundIndex != -1)
                    {
                        if (toChangeTriangles.ContainsKey(triangle))
                        {
                            toDeleteTriangles.Add(triangle, true);
                            continue;
                        }

                        toChangeTriangles.Add(triangle, foundIndex);
                    }
                }
            }

            // Increase size of arrays
            if (verts.Length <= head.index)
            {
                verts.Resize(head.index + 1, false, true);
                normals.Resize(head.index + 1, false, true);
                uvs.Resize(head.index + 1, false, true);
            }

            // Insert new vertex, normals, uvs
            Vector3[] meanVerts = new Vector3[head.children.Length];
            Vector3[] meanNormals = new Vector3[head.children.Length];
            Vector2[] meanUvs = new Vector2[head.children.Length];
            for (int i = 0; i < head.children.Length; i++)
            {
                meanVerts[i] = verts.Data[head.children[i].index];
                meanNormals[i] = normals.Data[head.children[i].index];
                meanUvs[i] = uvs.Data[head.children[i].index];
                indicesToDelete.Add(head.children[i].index);
            }

            verts.Data[head.index] = Vec3ArtMean(meanVerts);
            normals.Data[head.index] = Vec3ArtMean(meanNormals);
            uvs.Data[head.index] = Vec2ArtMean(meanUvs);

            // Cleaning triangles
            foreach (Triangle triangle in triangels.ToList())
            {
                if (toDeleteTriangles.ContainsKey(triangle))
                {
                    triangels.Remove(triangle);
                    continue;
                }

                if (toChangeTriangles.ContainsKey(triangle))
                {
                    triangle.data[toChangeTriangles[triangle]] = head.index;
                }
            }
        }
    }
    #endregion


    #region Calculating methods
    private static Vector3 Vec3ArtMean(Vector3[] vectors)
    {
        if (vectors == null) { return Vector3.zero; }
        if (vectors.Length == 1) { return vectors[0]; }

        Vector3 result = Vector3.zero;
        foreach (Vector3 vec in vectors)
        {
            result.x += vec.x;
            result.y += vec.y;
            result.z += vec.z;
        }
        return result / vectors.Length;
    }

    private static Vector2 Vec2ArtMean(Vector2[] vectors)
    {
        if (vectors == null) { return Vector2.zero; }
        if (vectors.Length == 1) { return vectors[0]; }

        Vector2 result = Vector2.zero;
        foreach (Vector2 vec in vectors)
        {
            result.x += vec.x;
            result.y += vec.y;
        }
        return result / vectors.Length;
    }
    #endregion
}
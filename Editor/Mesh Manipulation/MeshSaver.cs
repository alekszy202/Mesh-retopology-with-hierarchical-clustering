using UnityEngine;

public static class MeshSaver
{
    #region Parameters
    public static string meshPath = "Assets/Meshes/";
    #endregion


    #region Saving methods
    public static void SaveMeshAsset(Object asset, string meshName, string saveAssetsPath, bool fileOverride)
    {
        if (string.IsNullOrEmpty(meshName))
        {
            meshName = "unnamed";
        }

        meshName = IOUtils.MakeSafeFileName(meshName);
        saveAssetsPath = IOUtils.MakeSafeRelativePath(saveAssetsPath);

        string saveAssetPath = string.Format("{0}/{1}.mesh", saveAssetsPath, meshName);
        SaveAsset(asset, saveAssetPath, fileOverride);
    }

    public static void SaveAsset(Object asset, string path, bool fileOverride)
    {
        IOUtils.CreateParentDirectory(path);
        if (!fileOverride)
        {
            // Make sure that there is no asset with the same path already
            path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);
        }

        UnityEditor.AssetDatabase.CreateAsset(asset, path);

        var assetImporter = UnityEditor.AssetImporter.GetAtPath(path);
        if (assetImporter != null)
        {
            assetImporter.SaveAndReimport();
        }
        else
        {
            Debug.LogWarningFormat(asset, "Could not find asset importer for recently created asset, so could not mark it properly: {0}", path);
        }
    }
    #endregion


    #region API
    public static void SaveMesh(Object mesh, string meshName, bool fileOverride)
    {
        SaveMeshAsset(mesh, meshName, meshPath, fileOverride);
    }

    public static void SaveMesh(Object mesh, string meshName, float simplificationPercent, bool fileOverride)
    {
        string finalMeshName = $"{meshName}_{(int)simplificationPercent}%";
        SaveMeshAsset(mesh, finalMeshName, meshPath, fileOverride);
    }

    public static void SaveMesh(Object mesh, string meshName, ClusteringAffinity affinity, ClusteringLinkage linkage, string parameterName, int parameterValue, bool fileOverride)
    {
        string finalMeshName = $"{meshName}_{affinity}_{linkage}_{parameterName}:{parameterValue}";
        SaveMeshAsset(mesh, finalMeshName, meshPath, fileOverride);
    }
    #endregion
}
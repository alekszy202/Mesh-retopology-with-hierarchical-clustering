using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

internal static class IOUtils
{
    internal static string MakeSafeRelativePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        path = path.Replace('\\', '/').Trim('/');

        if (Path.IsPathRooted(path))
        {
            throw new ArgumentException("The path cannot be rooted.", "path");
        }

        // Make the path safe
        string[] pathParts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < pathParts.Length; i++)
        {
            pathParts[i] = MakeSafeFileName(pathParts[i]);
        }
        return string.Join("/", pathParts);
    }

    internal static string MakeSafeFileName(string name)
    {
        char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

        StringBuilder sb = new StringBuilder(name.Length);
        bool lastWasInvalid = false;
        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            if (!invalidFileNameChars.Contains(c))
            {
                sb.Append(c);
            }
            else if (!lastWasInvalid)
            {
                lastWasInvalid = true;
                sb.Append('_');
            }
        }
        return sb.ToString();
    }

#if UNITY_EDITOR
    internal static void CreateParentDirectory(string path)
    {
        int lastSlashIndex = path.LastIndexOf('/');
        if (lastSlashIndex != -1)
        {
            string parentPath = path.Substring(0, lastSlashIndex);
            if (!UnityEditor.AssetDatabase.IsValidFolder(parentPath))
            {
                lastSlashIndex = parentPath.LastIndexOf('/');
                if (lastSlashIndex != -1)
                {
                    string folderName = parentPath.Substring(lastSlashIndex + 1);
                    string folderParentPath = parentPath.Substring(0, lastSlashIndex);

                    CreateParentDirectory(parentPath);
                    UnityEditor.AssetDatabase.CreateFolder(folderParentPath, folderName);
                }
                else
                {
                    UnityEditor.AssetDatabase.CreateFolder(string.Empty, parentPath);
                }
            }
        }
    }

    internal static bool DeleteEmptyDirectory(string path)
    {
        bool deletedAllSubFolders = true;
        string[] subFolders = UnityEditor.AssetDatabase.GetSubFolders(path);
        for (int i = 0; i < subFolders.Length; i++)
        {
            if (!DeleteEmptyDirectory(subFolders[i]))
            {
                deletedAllSubFolders = false;
            }
        }

        if (!deletedAllSubFolders) { return false; }
        else if (!UnityEditor.AssetDatabase.IsValidFolder(path)) { return true; }

        string[] assetGuids = UnityEditor.AssetDatabase.FindAssets(string.Empty, new string[] { path });
        if (assetGuids.Length > 0) { return false; }

        return UnityEditor.AssetDatabase.DeleteAsset(path);
    }

    internal static string GetDirectoryFromPath(string path)
    {
        if (path.LastIndexOf('/') != -1)
            return path.Substring(0, path.LastIndexOf('/'));

        if (path.LastIndexOf('\\') != -1)
            return path.Substring(0, path.LastIndexOf('\\'));

        return string.Empty;
    }
#endif
}
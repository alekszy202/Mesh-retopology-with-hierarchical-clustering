using System;
using System.Globalization;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class VertexSaver
{
    #region MyRegion
    private Vector3[] threadMeshData;
    private string threadOutputPath;

    private bool threadFinished = false;
    private bool threadSucceeded = false;
    private string threadErrorMessage = string.Empty;
    #endregion


    #region Initialization methods
    private NumberFormatInfo InitializeNumberFormatInfo()
    {
        NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
        numberFormatInfo.NumberDecimalSeparator = ".";
        return numberFormatInfo;
    }
    #endregion


    #region Save vertices methods
    public bool SaveVerticesToCSVWithWindow(Mesh mesh, bool waitForThread)
    {
        string path = EditorUtility.SaveFilePanel("Save vertices", "", "Vertices", "csv");
        return SaveVerticesToCSV(mesh, path, waitForThread);
    }

    public bool SaveVerticesToCSV(Mesh mesh, string path, bool waitForThread)
    {
        Logger.LogInfo($"Saving vertices of mesh \"{mesh.name}.mesh\" to {path}");

        threadMeshData = mesh.vertices;
        threadOutputPath = path;

        Thread writingThread = new Thread(ThreadMeshSave);
        writingThread.Start();

        if (waitForThread)
        {
            while (!threadFinished) { }
            if (!threadSucceeded)
            {
                Logger.LogError(threadErrorMessage);
                return false;
            }
        }

        Logger.LogInfo($"Finished saving vertices of mesh \"{mesh.name}.mesh\" to {path}");
        return true;
    }

    private void ThreadMeshSave()
    {
        threadFinished = false;
        threadSucceeded = false;
        NumberFormatInfo numberFormatInfo = InitializeNumberFormatInfo();

        string directory = IOUtils.GetDirectoryFromPath(threadOutputPath);
        if (!Directory.Exists(directory))
        {
            threadErrorMessage = $"Directory \"{directory}\" does not exist!";
            threadSucceeded = false;
            threadFinished = true;
            return;
        }

        using (StreamWriter sw = new StreamWriter(threadOutputPath))
        {
            for (int i = 0; i < threadMeshData.Length; i++)
            {
                string x = Math.Round(Convert.ToDouble(threadMeshData[i].x), 8).ToString(numberFormatInfo);
                string y = Math.Round(Convert.ToDouble(threadMeshData[i].y), 8).ToString(numberFormatInfo);
                string z = Math.Round(Convert.ToDouble(threadMeshData[i].z), 8).ToString(numberFormatInfo);

                if (i < threadMeshData.Length - 1) { sw.WriteLine($"{x}, {y}, {z}"); }
                else { sw.Write($"{x}, {y}, {z}"); }
            }
        }
        threadFinished = true;
        threadSucceeded = true;
    }
    #endregion
}
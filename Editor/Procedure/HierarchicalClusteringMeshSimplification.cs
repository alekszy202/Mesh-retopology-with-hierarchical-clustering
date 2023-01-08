using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class HierarchicalClusteringMeshSimplification : MonoBehaviour
{
    #region Parameters
    [Header("References")]
    [SerializeField] private MeshFilter meshFilter;

    [Header("Settings")]
    public bool overrideMesh = true;
    public bool showCharts = false;
    public bool useAdvancedSettings = false;

    // Mesh settings
    [SerializeField] private string customMeshName = string.Empty;

    // Clustering settings
    public bool useDistance = true;

    [Range(0, 100)]
    [SerializeField] private float simplificationPercent = 90f;
    [SerializeField] private uint nClusters = 100;
    [SerializeField] private uint distanceThreshold = 100;

    [SerializeField] private ClusteringAffinity affinity = ClusteringAffinity.euclidean;
    [SerializeField] private ClusteringLinkage linkage = ClusteringLinkage.ward;

    private string packagePath = string.Empty;
    private SimplificationPaths paths;
    private PythonVenvBuilder venvBuilder;
    #endregion


    #region Initialization methods
    private void Start()
    {
        InitializePackagePath();
        InitializePaths(this.packagePath);
        InitializeVenvBuilder(this.packagePath);
    }

    private void InitializePackagePath()
    {
        packagePath = Path.GetFullPath("Packages/com.polsl.retopology-hierarchical-clustering");
    }

    private void InitializePaths(string packagePath)
    {
        paths = new SimplificationPaths(
            $"{packagePath}/brt-project/data/vertices.csv",
            $"{packagePath}/brt-project/data/parameters.json",
            $"{packagePath}/brt-project/data/result.csv",
            new List<string>()
            {
                "venv\\Scripts\\activate",
                $"cd {packagePath}\\brt-project",
                "py main.py"
            }
        );
    }

    private void InitializeVenvBuilder(string packagePath)
    {
        venvBuilder = new PythonVenvBuilder(".", @$"{packagePath}\\brt-project");
    }
    #endregion


    #region Simplification methods
    public void Simplify()
    {
        if (packagePath == string.Empty) { InitializePackagePath(); }
        if (paths == null) { InitializePaths(this.packagePath); }
        if (venvBuilder == null) { InitializeVenvBuilder(this.packagePath); }

        if (!Application.isEditor)
        {
            Logger.LogError("Hierarchical clustering retopology can't be executed in builded application");
            return;
        }

        // 0.Check python virtual environment
        if (!venvBuilder.IsVenvAvaliable())
        {
            Logger.LogError("No virtual python environment detected");
            string header = "Create venv";
            string message = "Do you want to create virtual environment for python project? It is essential for the proper performance of the system. \nMake sure you have installed python version 3.6 or 3.7";

            if (venvBuilder.OpenVenvBuildDialog(header, message))
                Logger.LogInfo("Installation of a virtual python environment has been started");
            else
                Logger.LogInfo("Installation of a virtual python environment has been canceled");
            return;
        }

        Logger.LogHeader("Hierarchical Clustering Mesh Simplification");

        // 1.Save mesh vertices to file
        Logger.LogTask("Save mesh vertices to file", 1, 6);
        VertexSaver vertexSaver = new VertexSaver();
        bool status = vertexSaver.SaveVerticesToCSV(meshFilter.sharedMesh, paths.vertexInputPath, true);
        if (!status) { return; }

        // 2.Save python parameters to file
        Logger.LogTask("Save python parameters to file", 2, 6);
        ClusteringParametersJsonParser parameters = GenerateParser(meshFilter.sharedMesh.vertices.Length);
        status = SaveParametersToJson(paths.parametersInputPath, parameters);
        if (!status) { return; }

        // 3.Execute python project
        Logger.LogTask("Execute python project", 3, 6);
        status = ExecutePythonScript(paths.PythonArguments);
        if (!status) { return; }

        // 4.Recreate clusters tree based on result
        Logger.LogTask("Recreate clusters tree based on result", 4, 6);
        ResultReader resultReader = new ResultReader(meshFilter.sharedMesh.vertices.Length, paths.resultPath);
        ClusterNode[] nodes = resultReader.RootList();
        if (nodes == null) { return; }

        // 5.Simplify mesh
        Logger.LogTask("Simplify mesh", 5, 6);
        Mesh newMesh = SimplifyMesh.Simplify(meshFilter.sharedMesh, nodes);
        string meshName = meshFilter.sharedMesh.name;
        meshFilter.mesh = newMesh;

        // 6.Save simplified mesh
        Logger.LogTask("Save simplified mesh", 6, 6);
        MeshSavingProcedure(newMesh, meshName);

        Logger.LogInfo("Simplification operation performed successfully");
    }
    #endregion


    #region Saving parameters methods
    private ClusteringParametersJsonParser GenerateParser(int numberOfVertices)
    {
        if (useAdvancedSettings)
            return GenerateAdvancedParser();
        else
            return GenerateRegularParser(numberOfVertices);
    }

    private ClusteringParametersJsonParser GenerateRegularParser(int numberOfVertices)
    {
        int numberOfClusters = (int)(numberOfVertices * simplificationPercent / 100);
        return new ClusteringParametersJsonParser(numberOfClusters, null, ClusteringAffinity.euclidean.ToString(), ClusteringLinkage.ward.ToString(), showCharts);
    }

    private ClusteringParametersJsonParser GenerateAdvancedParser()
    {
        if (useDistance)
            return new ClusteringParametersJsonParser(null, (int)distanceThreshold, affinity.ToString(), linkage.ToString(), showCharts);
        else
            return new ClusteringParametersJsonParser((int)nClusters, null, affinity.ToString(), linkage.ToString(), showCharts);
    }

    private bool SaveParametersToJson(string path, ClusteringParametersJsonParser clusteringParameters)
    {
        Logger.LogInfo($"Saving clustering parameters to {path}");

        string directory = IOUtils.GetDirectoryFromPath(path);
        if (!Directory.Exists(directory))
        {
            Logger.LogError($"Directory \"{directory}\" does not exist!");
            return false;
        }

        string parameters = clusteringParameters.GenerateJsonSource();
        File.WriteAllText(path, parameters);

        Logger.LogInfo($"Finished saving clustering parameters to {path}");
        return true;
    }
    #endregion


    #region Python execution methods
    private bool ExecutePythonScript(string arguments)
    {
        bool exitFlagFound = false;
        string programPath = "cmd.exe";
        Logger.LogInfo("Started executing python script");

        try
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = programPath,
                Arguments = $"/k {arguments}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            System.Diagnostics.Process process = new System.Diagnostics.Process { StartInfo = startInfo };
            process.Start();

            string output = "content";
            while ((output != null || !process.HasExited) && !exitFlagFound)
            {
                output = process.StandardOutput.ReadLine();

                if (!string.IsNullOrEmpty(output))
                {
                    if (output == "SUCCEEDED")
                    {
                        exitFlagFound = true;
                        Logger.LogInfo("Finished executing python script");
                        process.Close();
                        return true;
                    }

                    if (output == "ERROR")
                    {
                        exitFlagFound = true;
                        Logger.LogError(process.StandardOutput.ReadLine());
                        process.Close();
                        return false;
                    }

                    Logger.LogPython(output);
                }
            }
            process.Close();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.GetType().ToString());
            return false;
        }

        Logger.LogError("Unexpected python connection error");
        return false;
    }
    #endregion


    #region Mesh saving methods
    private void MeshSavingProcedure(Mesh newMesh, string meshName)
    {
        if (overrideMesh)
        {
            MeshSaver.SaveMesh(newMesh, meshName, true);
        }
        else
        {
            if (string.IsNullOrEmpty(customMeshName))
            {
                if (useAdvancedSettings)
                {
                    if (useDistance)
                        MeshSaver.SaveMesh(newMesh, meshName, affinity, linkage, "distanceThreshold", (int)distanceThreshold, false);
                    else
                        MeshSaver.SaveMesh(newMesh, meshName, affinity, linkage, "nClusters", (int)distanceThreshold, false);
                }
                else
                    MeshSaver.SaveMesh(newMesh, meshName, simplificationPercent, false);
            }
            else
                MeshSaver.SaveMesh(newMesh, customMeshName, false);
        }
    }
    #endregion
}
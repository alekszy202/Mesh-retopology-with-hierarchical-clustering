using System.Collections.Generic;

public class SimplificationPaths
{
    public string vertexInputPath = "./Packages/Hierarchical Clustering Mesh Simplification/brt-project/vertices.csv";
    public string parametersInputPath = "./Packages/Hierarchical Clustering Mesh Simplification/brt-project/parameters.json";
    public string resultPath = "./Packages/Hierarchical Clustering Mesh Simplification/brt-project/result.csv";

    public List<string> pythonExecutionCommands = new List<string>();

    public SimplificationPaths(string vertexInputPath, string parametersInputPath, string resultPath, List<string> pythonExecutionCommands)
    {
        this.vertexInputPath = vertexInputPath;
        this.parametersInputPath = parametersInputPath;
        this.resultPath = resultPath;
        this.pythonExecutionCommands = pythonExecutionCommands;
    }

    public string PythonArguments
    {
        get
        {
            string result = string.Empty;
            if (pythonExecutionCommands == null || pythonExecutionCommands.Count == 0)
            {
                return result;
            }

            result = pythonExecutionCommands[0];
            for (int i = 1; i < pythonExecutionCommands.Count; i++)
            {
                result += $"&{pythonExecutionCommands[i]}";
            }
            return result;
        }
    }
}
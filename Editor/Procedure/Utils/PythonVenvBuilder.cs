using System.Diagnostics;
using System.IO;
using UnityEditor;

public class PythonVenvBuilder
{
    private string venvPath;
    private string pythonProjectPath;

    public PythonVenvBuilder(string venvPath, string pythonProjectPath)
    {
        this.venvPath = venvPath;
        this.pythonProjectPath = pythonProjectPath;
    }

    public bool IsVenvAvaliable()
    {
        return Directory.Exists($"{venvPath}\\venv");
    }

    public bool OpenVenvBuildDialog(string header, string message)
    {
        bool result = EditorUtility.DisplayDialog(header, message, "Yes", "No");
        if (result)
        {
            BuildVenv();
        }
        return result;
    }

    public void BuildVenv()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/k " +
            $"echo Installation of the virtual python environment started&" +
            $"echo Don't close this window until it's finished&" +
            $"cd {venvPath}&" +
            $"py -m venv venv&" +
            $"venv\\Scripts\\activate&" +
            $"cd {pythonProjectPath}&" +
            $"pip install -r requirements.txt&" +
            $"python -m pip install --upgrade pip&" +
            $"echo The virtual python environment has been installed"
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
    }
}
using UnityEngine;

public static class Logger
{
    public static void LogHeader(string header)
    {
        Debug.Log($"--->>> <b><color=red>{header}</color></b> <<<---");
    }

    public static void LogTask(string taskName, int taskNumber, int maxTaskNumber)
    {
        Debug.Log($"<b><color=red>Task [{taskNumber}/{maxTaskNumber}]:</color></b> {taskName}");
    }

    public static void LogInfo(string message)
    {
        LogColorMessage("#00ff00", "Info", message);
    }

    public static void LogError(string message)
    {
        LogColorMessage("red", "Error", message);
    }

    public static void LogPython(string message)
    {
        LogColorMessage("yellow", "Python", message);
    }

    public static void LogColorMessage(string color, string header, string message)
    {
        Debug.Log($"<color={color}>{header}:</color> {message}");
    }
}
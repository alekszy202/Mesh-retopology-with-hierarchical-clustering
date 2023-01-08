using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(HierarchicalClusteringMeshSimplification))]
public class HierarchicalClusteringMeshSimplificationEditor : Editor
{
    private SerializedProperty meshFilter;
    private SerializedProperty customMeshName;

    private SerializedProperty simplificationPercent;
    private SerializedProperty nClusters;
    private SerializedProperty distanceThreshold;
    private SerializedProperty affinity;
    private SerializedProperty linkage;

    private void OnEnable()
    {
        meshFilter = serializedObject.FindProperty("meshFilter");
        customMeshName = serializedObject.FindProperty("customMeshName");

        simplificationPercent = serializedObject.FindProperty("simplificationPercent");
        nClusters = serializedObject.FindProperty("nClusters");
        distanceThreshold = serializedObject.FindProperty("distanceThreshold");
        affinity = serializedObject.FindProperty("affinity");
        linkage = serializedObject.FindProperty("linkage");
    }

    public override void OnInspectorGUI()
    {
        HierarchicalClusteringMeshSimplification myScript = target as HierarchicalClusteringMeshSimplification;

        EditorGUILayout.Space();
        if (GUILayout.Button("Simplify mesh"))
        {
            myScript.Simplify();
        }

        serializedObject.Update();

        Header("References");
        EditorGUILayout.ObjectField(meshFilter);

        Header("Settings");
        myScript.overrideMesh = EditorGUILayout.Toggle("Override mesh", myScript.overrideMesh);
        myScript.showCharts = EditorGUILayout.Toggle("Show charts", myScript.showCharts);
        myScript.useAdvancedSettings = EditorGUILayout.Toggle("Use advanced settings", myScript.useAdvancedSettings);

        EditorGUILayout.Space();
        if (!myScript.overrideMesh)
        {
            Header("Mesh settings");
            EditorGUILayout.PropertyField(customMeshName);
            EditorGUILayout.HelpBox("Leave empty to render name with clustering statistics", MessageType.Info);
        }

        EditorGUILayout.Space();
        if (myScript.useAdvancedSettings)
        {
            Header("Advanced clustering settings");
            myScript.useDistance = EditorGUILayout.Toggle("Use distance", myScript.useDistance);
            if (myScript.useDistance)
                EditorGUILayout.PropertyField(distanceThreshold);
            else
                EditorGUILayout.PropertyField(nClusters);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(affinity);
            EditorGUILayout.PropertyField(linkage);
        }
        else
        {
            Header("Clustering settings");
            EditorGUILayout.PropertyField(simplificationPercent);
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void Header(string message)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(message, EditorStyles.boldLabel);
    }
}
#endif
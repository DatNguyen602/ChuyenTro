#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexGridGenerator))]
public class HexGridGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HexGridGenerator generator = (HexGridGenerator)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Hex Grid"))
        {
            generator.GenerateHexGrid();
        }
    }
}
#endif

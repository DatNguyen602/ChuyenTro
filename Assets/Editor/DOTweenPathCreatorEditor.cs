#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DOTweenPathCreator))]
public class DOTweenPathCreatorEditor : Editor
{
    private DOTweenPathCreator creator;
    private SerializedProperty waypointsProp;

    private void OnEnable()
    {
        creator = (DOTweenPathCreator)target;
        waypointsProp = serializedObject.FindProperty("waypoints");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Hiện list Waypoints (position + pauseDuration)
        EditorGUILayout.PropertyField(waypointsProp, true);
        EditorGUILayout.Space();

        // Nút thêm waypoint
        if (GUILayout.Button("Add Waypoint at End"))
        {
            Undo.RecordObject(creator, "Add Waypoint");
            var last = creator.waypoints[creator.waypoints.Count - 1];
            creator.waypoints.Add(new DOTweenPathCreator.Waypoint
            {
                position = last.position + Vector3.right,
                pauseDuration = last.pauseDuration
            });
            EditorUtility.SetDirty(creator);
        }

        // Nút xóa tất cả
        if (creator.waypoints.Count > 2 && GUILayout.Button("Clear All Waypoints"))
        {
            if (EditorUtility.DisplayDialog("Clear Waypoints", "Xóa toàn bộ waypoints?", "Yes", "No"))
            {
                Undo.RecordObject(creator, "Clear Waypoints");
                creator.waypoints.Clear();
                EditorUtility.SetDirty(creator);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        Handles.color = creator.lineColor;
        Transform t = creator.transform;

        // Vẽ đường nối giữa các waypoint
        for (int i = 0; i < creator.waypoints.Count - 1; i++)
        {
            Vector3 a = t.TransformPoint(creator.waypoints[i].position);
            Vector3 b = t.TransformPoint(creator.waypoints[i + 1].position);
            Handles.DrawLine(a, b);
        }

        // Vẽ handle & cho di chuyển từng waypoint
        for (int i = 0; i < creator.waypoints.Count; i++)
        {
            var wp = creator.waypoints[i];
            Vector3 worldPos = t.TransformPoint(wp.position);
            float size = HandleUtility.GetHandleSize(worldPos) * creator.pointHandleSize;
            Handles.color = Color.yellow;

            // Chọn point khi click
            if (Handles.Button(worldPos, Quaternion.identity, size, size * creator.pointPickSize, Handles.SphereHandleCap))
                Selection.activeObject = creator;

            // Di chuyển bằng gizmo
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(worldPos, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(creator, "Move Waypoint");
                wp.position = t.InverseTransformPoint(newPos);
                EditorUtility.SetDirty(creator);
            }

            // Hiện nhãn pauseDuration
            Handles.Label(worldPos + Vector3.up * size * 1.2f, $"Pause: {wp.pauseDuration:F2}s");
        }
    }
}
#endif
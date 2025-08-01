#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DOTweenPathCreator))]
public class DOTweenPathCreatorEditor : Editor
{
    private DOTweenPathCreator creator;
    private SerializedProperty waypointsProp;

    [MenuItem("GameObject/2D Object/Path Creator", false, 10)]
    public static void CreatePathCreator()
    {
        GameObject go = new GameObject("Path Creator");
        GameObject picked = Selection.activeGameObject;
        if (picked != null)
        {
            go.transform.SetParent(picked.transform);
        }
        DOTweenPathCreator dOTweenPathCreator = go.AddComponent<DOTweenPathCreator>();
        Selection.activeGameObject = go;
        GameObject flow = new GameObject("ObjectFlow");
        flow.transform.SetParent(go.transform);
        PathFollowerDOTween pathFollowerDOTween = flow.AddComponent<PathFollowerDOTween>();
        pathFollowerDOTween.pathCreator = dOTweenPathCreator;
        GameObject avatar = new GameObject("Avatar");
        avatar.transform.SetParent(flow.transform);
        SpriteRenderer sr = avatar.AddComponent<SpriteRenderer>();
        Texture2D tex = EditorGUIUtility.whiteTexture;
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        sr.sprite = sprite;
    }

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
            Handles.Label(worldPos + Vector3.up /** size * 2f*/
                , $"Pause\n{wp.pauseDuration.from:F2}~{wp.pauseDuration.to:F2}s");
        }
    }
}

[CustomPropertyDrawer(typeof(ObjectRan))]
public class ObjectRanDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Tính toán layout
        EditorGUI.BeginProperty(position, label, property);

        var fromProp = property.FindPropertyRelative("from");
        var toProp = property.FindPropertyRelative("to");

        float fieldWidth = 98;

        Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
        EditorGUI.LabelField(labelRect, label);

        float xStart = 156;

        // From field
        Rect fromFieldRect = new Rect(xStart, position.y, fieldWidth, position.height);
        EditorGUI.PropertyField(fromFieldRect, fromProp, GUIContent.none);

        Rect toFieldRect = new Rect(256, position.y, fieldWidth, position.height);
        EditorGUI.PropertyField(toFieldRect, toProp, GUIContent.none);

        EditorGUI.EndProperty();
    }
}
#endif
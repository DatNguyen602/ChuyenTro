using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class TileSnapHandler
{
    static TileSnapHandler()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView view)
    {
        Event e = Event.current;
        if (e == null || e.type != EventType.MouseUp) return;

        GameObject selected = Selection.activeGameObject;
        if (selected == null) return;

        TileContent content = selected.GetComponent<TileContent>();
        if (content == null) return;

        Vector2 origin = content.transform.position;

        Collider2D hitCol = Physics2D.OverlapPoint(origin);

        if (hitCol != null)
        {
            HexTile hex = hitCol.GetComponent<HexTile>();
            if (hex != null)
            {
                selected.transform.position = hex.transform.position;
                return;
            }
        }

        if (PrefabUtility.GetPrefabAssetType(selected) == PrefabAssetType.NotAPrefab)
        {
            Object.DestroyImmediate(selected);
        }

    }

}

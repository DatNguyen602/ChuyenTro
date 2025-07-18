using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class GridItemCreator : EditorWindow
{
    private string itemName = "New Item";
    private string id = "item_001";
    private Sprite sprite;
    private float spriteScale = 1f;
    private Vector2 spriteOffsetPercent = Vector2.zero;

    private GridItem selectedItem = null;
    private bool isEditing = false;

    private HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();

    private int cellSize = 20;
    private const int gridRadius = 6;
    private GUIStyle centerLabel;

    [MenuItem("Tools/Grid Item Creator")]
    public static void ShowWindow()
    {
        GetWindow<GridItemCreator>("🧩 Grid Item Creator");
    }

    private void OnEnable()
    {
        try
        {
            centerLabel = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12
            };
        }
        catch { }
    }

    private void OnGUI()
    {
        GUILayout.Space(8);
        GUILayout.Label("📦 Thông tin vật phẩm", EditorStyles.boldLabel);
        itemName = EditorGUILayout.TextField("Tên", itemName);
        id = EditorGUILayout.TextField("ID", id);

        GUILayout.Space(10);
        GUILayout.Label("🖼️ Cài đặt Sprite", EditorStyles.boldLabel);
        sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", sprite, typeof(Sprite), false);
        spriteScale = EditorGUILayout.Slider("Kích thước (số ô)", spriteScale, 0.1f, 5f);
        spriteOffsetPercent = EditorGUILayout.Vector2Field("Dịch chuyển (%)", spriteOffsetPercent);

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("🎯 Grid", EditorStyles.boldLabel);
        cellSize = Mathf.RoundToInt(EditorGUILayout.Slider(cellSize, 5, 50, GUILayout.MaxWidth(312)));
        GUILayout.EndHorizontal();
        Rect previewRect = GUILayoutUtility.GetRect(cellSize * gridRadius * 2, cellSize * gridRadius * 2);
        DrawGrid(previewRect);
        GUILayout.EndVertical();

        GUILayout.Space(10);
        GUILayout.BeginVertical();
        GUILayout.Label("📝 Chỉnh sửa vật phẩm có sẵn", EditorStyles.boldLabel);

        // Lấy tất cả GridItem trong project
        string[] guids = AssetDatabase.FindAssets("t:GridItem");
        List<GridItem> gridItems = new List<GridItem>();
        List<string> options = new List<string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GridItem item = AssetDatabase.LoadAssetAtPath<GridItem>(path);
            if (item != null)
            {
                gridItems.Add(item);
                options.Add(item.name);
            }
        }

        // Dropdown chọn
        int selectedIndex = selectedItem != null ? gridItems.IndexOf(selectedItem) : -1;
        int newIndex = EditorGUILayout.Popup("Chọn Item", selectedIndex, options.ToArray());

        if (newIndex != selectedIndex && newIndex >= 0 && newIndex < gridItems.Count)
        {
            selectedItem = gridItems[newIndex];
            LoadItemData(selectedItem);
        }

        GUILayout.Space(10);
        if (!isEditing)
        {
            if (GUILayout.Button("✅ Tạo GridItem", GUILayout.Height(35)))
                CreateItemAsset();
        }
        else
        {
            if (GUILayout.Button("💾 Lưu thay đổi", GUILayout.Height(35)))
                SaveEditedItem();

            if (GUILayout.Button("↩️ Hủy chỉnh sửa"))
                ClearForm();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    private void DrawGrid(Rect rect)
    {
        Handles.BeginGUI();
        Vector2 center = rect.center;
        Vector2 mousePos = Event.current.mousePosition;

        EditorGUI.DrawRect(rect, new Color(0.95f, 0.95f, 0.95f, 1f));

        if (Event.current.type == EventType.MouseDown && rect.Contains(mousePos))
        {
            Vector2 local = mousePos - center;
            int x = Mathf.FloorToInt(local.x / cellSize);
            int y = -Mathf.FloorToInt(local.y / cellSize);
            Vector2Int clickedCell = new Vector2Int(x, y);

            if (occupiedCells.Contains(clickedCell))
                occupiedCells.Remove(clickedCell);
            else
                occupiedCells.Add(clickedCell);

            Repaint();
            Event.current.Use();
        }

        // Vẽ sprite
        if (sprite != null && sprite.texture != null)
        {
            try
            {
                Rect spriteRect = sprite.rect;
                Texture2D tex = sprite.texture;

                float pixelPerUnit = sprite.pixelsPerUnit == 0 ? 100 : sprite.pixelsPerUnit;
                Vector2 spriteWorldSize = new Vector2(spriteRect.width, spriteRect.height) / pixelPerUnit;
                Vector2 normalizedSize = spriteWorldSize / Mathf.Max(spriteWorldSize.x, spriteWorldSize.y); // giữ tỉ lệ

                Vector2 screenSize = normalizedSize * spriteScale * cellSize;

                Vector2 offsetPixels = new Vector2(
                    spriteOffsetPercent.x * cellSize + cellSize * 0.5f,
                    -spriteOffsetPercent.y * cellSize + cellSize * 0.5f
                );

                Rect drawRect = new Rect(
                    center.x - screenSize.x / 2 + offsetPixels.x,
                    center.y - screenSize.y / 2 + offsetPixels.y,
                    screenSize.x,
                    screenSize.y
                );

                Rect uv = new Rect(
                    spriteRect.x / tex.width,
                    spriteRect.y / tex.height,
                    spriteRect.width / tex.width,
                    spriteRect.height / tex.height
                );

                GUI.DrawTextureWithTexCoords(drawRect, tex, uv);
            }
            catch { }
        }

        // Vẽ lưới
        for (int x = -gridRadius; x <= gridRadius; x++)
        {
            for (int y = -gridRadius; y <= gridRadius; y++)
            {
                Vector2 pos = new Vector2(center.x + x * cellSize, center.y - y * cellSize);
                Rect cell = new Rect(pos.x, pos.y, cellSize, cellSize);
                Color color = (x == 0 || y == 0) ? Color.black : Color.gray;

                Handles.DrawSolidRectangleWithOutline(cell, Color.clear, color);

                Vector2Int cellCoord = new Vector2Int(x, y);
                if (occupiedCells.Contains(cellCoord))
                    Handles.DrawSolidRectangleWithOutline(cell, new Color(0.3f, 1f, 0.3f, 0.4f), Color.green);

                if (x == 0 && y == 0)
                    GUI.Label(cell, "0,0", centerLabel);
            }
        }

        Handles.EndGUI();
    }

    private void CreateItemAsset()
    {
        GridItem newItem = ScriptableObject.CreateInstance<GridItem>();
        newItem.itemName = itemName;
        newItem.id = id;
        newItem.sprite = sprite;
        newItem.spriteScale = spriteScale;
        newItem.spriteOffsetPercent = spriteOffsetPercent;
        newItem.occupiedCells = new List<Vector2Int>(occupiedCells);

        string folderPath = "Assets/Resources/GridItems";
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets", "GridItems");

        string path = $"{folderPath}/{id}.asset";
        AssetDatabase.CreateAsset(newItem, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newItem;
        Debug.Log($"✅ Tạo thành công GridItem: {path}");

        ClearForm();
    }

    private void SaveEditedItem()
    {
        if (selectedItem == null) return;

        selectedItem.itemName = itemName;
        selectedItem.id = id;
        selectedItem.sprite = sprite;
        selectedItem.spriteScale = spriteScale;
        selectedItem.spriteOffsetPercent = spriteOffsetPercent;
        selectedItem.occupiedCells = new List<Vector2Int>(occupiedCells);

        EditorUtility.SetDirty(selectedItem);
        AssetDatabase.SaveAssets();

        Debug.Log($"💾 Đã lưu chỉnh sửa cho GridItem: {selectedItem.name}");
        ClearForm();
    }

    private void LoadItemData(GridItem item)
    {
        itemName = item.itemName;
        id = item.id;
        sprite = item.sprite;
        spriteScale = item.spriteScale;
        spriteOffsetPercent = item.spriteOffsetPercent;
        occupiedCells = new HashSet<Vector2Int>(item.occupiedCells);
        isEditing = true;
        Repaint();
    }

    private void ClearForm()
    {
        itemName = "New Item";
        id = GenerateNextID(id);
        sprite = null;
        spriteScale = 1f;
        spriteOffsetPercent = Vector2.zero;
        occupiedCells.Clear();
        selectedItem = null;
        isEditing = false;
        Repaint();
    }

    private string GenerateNextID(string currentID)
    {
        string prefix = "item_";
        int number = 1;
        if (currentID.StartsWith(prefix))
        {
            string numPart = currentID.Substring(prefix.Length);
            if (int.TryParse(numPart, out int parsed)) number = parsed + 1;
        }
        return prefix + number.ToString("D3");
    }
}

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ClearPlayerPrefsButton
{
    [MenuItem("Tools/Clear All PlayerPrefs")]
    public static void ClearAllPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog("Xóa PlayerPrefs?",
            "Bạn có chắc muốn xóa toàn bộ dữ liệu PlayerPrefs không?", "Xóa", "Hủy"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("✅ Đã xóa toàn bộ PlayerPrefs.");
        }
    }
}

#endif
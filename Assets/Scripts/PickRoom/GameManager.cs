using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SpriteRenderer selectedUniversity; // Cái SpriteRenderer trên nhân vật hoặc UI

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // giữ lại khi sang scene khác
    }

    private void Start()
    {
        // Lấy logo từ GameData
        if (GameData.Instance != null && GameData.Instance.selectedLogo != null)
        {
            selectedUniversity.sprite = GameData.Instance.selectedLogo;
        }
    }
}

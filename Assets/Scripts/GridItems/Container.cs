using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public static Container Instance { get; private set; }

    public Vector2Int size = new Vector2Int(1, 1);
    [SerializeField] private GameObject squarePrefab;

    private List<List<bool>> stateList = new List<List<bool>>();

    // Khoảng cách giữa các ô (nên nhỏ hơn với mobile)
    [SerializeField] private float spacing = 1f;

    // scale theo tỷ lệ màn hình
    [SerializeField] private bool autoScale = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        float scaleFactor = 1f;
        if (autoScale)
        {
            float screenRatio = (float)Screen.width / Screen.height;
            scaleFactor = Mathf.Min(1f, 10f / Mathf.Max(size.x, size.y));
        }

        for (int i = 0; i < size.x; i++)
        {
            stateList.Add(new List<bool>());
            for (int j = 0; j < size.y; j++)
            {
                stateList[i].Add(false);
                GameObject square = Instantiate(squarePrefab, transform);
                square.transform.localScale = Vector3.one * scaleFactor;

                // Vị trí tính toán cho màn hình di động (centered)
                float posX = (i - size.x / 2f + 0.5f) * spacing * scaleFactor;
                float posY = (j - size.y / 2f + 0.5f) * spacing * scaleFactor;
                square.transform.localPosition = new Vector3(posX, posY, 0);
            }
        }

        Camera.main.orthographicSize = size.x + 2;
    }

    public bool CheckState(Vector2 pos, List<Vector2Int> occupiedCells)
    {
        Vector2Int cell = new Vector2Int(Mathf.RoundToInt(pos.x + (size.x / 2)), Mathf.RoundToInt(pos.y + (size.y / 2)));
        if (cell.x < 0 || cell.x >= size.x || cell.y < 0 || cell.y >= size.y)
            return false;

        foreach (Vector2Int occupiedCell in occupiedCells)
        {
            int checkX = cell.x + Mathf.FloorToInt(occupiedCell.x);
            int checkY = cell.y + Mathf.FloorToInt(occupiedCell.y);

            if (checkX < 0 || checkX >= size.x || checkY < 0 || checkY >= size.y)
                return false;

            if (stateList[checkX][checkY])
                return false;
        }
        return true;
    }

    public void SetState(Vector2 pos, List<Vector2Int> occupiedCells, bool state = true)
    {
        Vector2Int cell = new Vector2Int(Mathf.RoundToInt(pos.x + (size.x / 2)), Mathf.RoundToInt(pos.y + (size.y / 2)));
        if (cell.x < 0 || cell.x >= size.x || cell.y < 0 || cell.y >= size.y)
            return;

        foreach (Vector2Int occupiedCell in occupiedCells)
        {
            int setX = cell.x + Mathf.FloorToInt(occupiedCell.x);
            int setY = cell.y + Mathf.FloorToInt(occupiedCell.y);

            if (setX < 0 || setX >= size.x || setY < 0 || setY >= size.y)
                continue;

            stateList[setX][setY] = state;
            transform.GetChild(setX * size.y + setY).GetComponent<SpriteRenderer>().color = state ? Color.gray : Color.white;
        }
    }
    public Vector2Int WorldToCell(Vector2 pos)
    {
        return new Vector2Int(
            Mathf.RoundToInt(pos.x + (size.x / 2)),
            Mathf.RoundToInt(pos.y + (size.y / 2))
        );
    }

}

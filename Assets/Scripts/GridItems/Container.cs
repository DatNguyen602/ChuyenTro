using System;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public static Container Instance { get; private set; }

    public Vector2Int size = new Vector2Int(1, 1);
    [SerializeField] private GameObject squarePrefab;
    private List<List<bool>> stateList = new List<List<bool>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < size.x; i++)
        {
            stateList.Add(new List<bool>());
            for (int j = 0; j < size.y; j++)
            {
                stateList[i].Add(false);
                GameObject square = Instantiate(squarePrefab, transform);
                //renderer.sprite = Resources.Load<Sprite>("Sprites/WhiteSquare");

                square.transform.position = new Vector3(i - (size.x / 2), j - (size.y / 2), 0);
                square.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public bool CheckState(Vector2 pos, List<Vector2Int> occupiedCells)     {
        Vector2Int cell = new Vector2Int(Mathf.RoundToInt(pos.x + (size.x / 2)), Mathf.RoundToInt(pos.y + (size.y / 2)));
        if (cell.x < 0 || cell.x >= size.x || cell.y < 0 || cell.y >= size.y)
            return false;
        
        foreach (Vector2Int occupiedCell in occupiedCells)
        {
            try
            {
                if(stateList[cell.x + Mathf.FloorToInt(occupiedCell.x)]
                    [cell.y + Mathf.FloorToInt(occupiedCell.y)])
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //Debug.Log(ex);
                //Debug.Log($"Cell: {cell}, OccupiedCell: {occupiedCell}");
                return false;
            }
        }
        return true;
    }

    public void SetState(Vector2 pos, List<Vector2Int> occupiedCells)
    {
        Vector2Int cell = new Vector2Int(Mathf.RoundToInt(pos.x + (size.x / 2)), Mathf.RoundToInt(pos.y + (size.y / 2)));
        if (cell.x < 0 || cell.x >= size.x || cell.y < 0 || cell.y >= size.y)
            return;

        foreach (Vector2Int occupiedCell in occupiedCells)
        {
            try
            {
                stateList[cell.x + Mathf.FloorToInt(occupiedCell.x)]
                    [cell.y + Mathf.FloorToInt(occupiedCell.y)] = true;
            }
            catch { }
        }
    }
}

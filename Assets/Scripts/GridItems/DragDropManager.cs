using System.Collections.Generic;
using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager instance;

    private GameObject objectDrag;
    public delegate void EndDragAction(GameObject gameObject, bool canDrag);
    public EndDragAction EndDrag;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void SetDrag(GameObject obj)
    {
        if(obj == null)
        {
            objectDrag = null;
            return;
        }
        if (!objectDrag) objectDrag = obj;

        GridItemObject gridItemObject = objectDrag.GetComponent<GridItemObject>();
        PolygonCollider2D poly = objectDrag.GetComponent<PolygonCollider2D>();

        List<Vector2[]> paths = new List<Vector2[]>();

        foreach (Vector2 pos in gridItemObject.gridItem.occupiedCellsStart)
        {
            Vector2[] square = new Vector2[] {
                pos + new Vector2(-0.5f, -0.5f),
                pos + new Vector2(-0.5f,  0.5f),
                pos + new Vector2( 0.5f,  0.5f),
                pos + new Vector2( 0.5f, -0.5f)
            };
            paths.Add(square);
        }

        poly.pathCount = paths.Count;
        for (int i = 0; i < paths.Count; i++)
        {
            poly.SetPath(i, paths[i]);
        }
    }

    void Update()
    {
        Vector3 inputPosition;
        bool released = GetInput(out inputPosition);

        if (released && objectDrag != null)
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(inputPosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            Vector2Int posCheck = new Vector2Int(-99, -99);

            if (hit.collider?.gameObject)
            {
                posCheck = new Vector2Int(Mathf.FloorToInt(hit.collider.transform.position.x), Mathf.FloorToInt(hit.collider.transform.position.y));
            }

            bool canDrag = Container.Instance.CheckState(posCheck,
                objectDrag.GetComponent<GridItemObject>().gridItem.occupiedCells);

            EndDrag?.Invoke(objectDrag, canDrag);

            if (canDrag)
            {
                if (hit.collider?.gameObject)
                    Container.Instance.SetState(
                        new Vector2Int(Mathf.FloorToInt(hit.collider.transform.position.x), Mathf.FloorToInt(hit.collider.transform.position.y)),
                        objectDrag.GetComponent<GridItemObject>().gridItem.occupiedCells);
                objectDrag.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }

            objectDrag.GetComponent<PolygonCollider2D>().enabled = true;
            objectDrag = null;
        }

        DragAction(inputPosition);
    }

    private void DragAction(Vector3 inputScreenPos)
    {
        if (!UtilsInput.IsValidMousePosition(inputScreenPos)) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(inputScreenPos);
        //worldPos.z = 0;

        if (objectDrag == null)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.CompareTag("ElConttainer"))
            {
                GridItemObject gridObj = hit.collider.gameObject.GetComponent<GridItemObject>();
                if (gridObj != null && Container.Instance.CheckState(
                        new Vector2Int(Mathf.FloorToInt(hit.collider.transform.position.x), Mathf.FloorToInt(hit.collider.transform.position.y)),
                        gridObj.gridItem.occupiedCells))
                {
                    SetDrag(hit.collider.gameObject);
                }
            }
        }
        else
        {
            objectDrag.GetComponent<PolygonCollider2D>().enabled = false;

            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.CompareTag("ElConttainer") &&
                Container.Instance.CheckState(
                    new Vector2Int(Mathf.FloorToInt(hit.collider.transform.position.x), Mathf.FloorToInt(hit.collider.transform.position.y)),
                    objectDrag.GetComponent<GridItemObject>().gridItem.occupiedCells))
            {
                objectDrag.transform.position = hit.collider.transform.position;
                objectDrag.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.green;
            }
            else
            {
                objectDrag.transform.position = worldPos;
                objectDrag.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.red;
            }
        }
    }

    /// <summary>
    /// Xử lý đầu vào từ chuột hoặc cảm ứng. Trả về tọa độ màn hình & true nếu vừa nhả.
    /// </summary>
    private bool GetInput(out Vector3 inputPos)
    {
        inputPos = Vector3.zero;

#if UNITY_EDITOR
        inputPos = Input.mousePosition;
        return Input.GetMouseButtonUp(0);
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputPos = touch.position;
            return touch.phase == TouchPhase.Ended;
        }
        return false;
#endif
    }
}

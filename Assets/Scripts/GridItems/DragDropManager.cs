using System.Collections.Generic;
using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager instance;

    private GameObject objectDrag;
    public delegate bool EndDragAction(GameObject gameObject, bool canDrag);
    public EndDragAction EndDrag;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private float timeSinceLastInput = 0f;
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

        foreach (Vector2 pos in gridItemObject.gridItem.occupiedCells)
        {
            Vector2[] square = new Vector2[] {
                pos + new Vector2(-0.499f, -0.499f),
                pos + new Vector2(-0.499f,  0.499f),
                pos + new Vector2( 0.499f,  0.499f),
                pos + new Vector2( 0.499f, -0.499f)
            };
            paths.Add(square);
        }

        poly.pathCount = paths.Count;
        for (int i = 0; i < paths.Count; i++)
        {
            poly.SetPath(i, paths[i]);
        }
        timeSinceLastInput = Time.time;
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
                objectDrag.GetComponent<GridItemObject>().gridItem.occupiedCellsStart);

            canDrag = EndDrag.Invoke(objectDrag, canDrag);

            if (canDrag)
            {
                if (hit.collider?.gameObject)
                    Container.Instance.SetState(
                        new Vector2Int(Mathf.FloorToInt(hit.collider.transform.position.x), Mathf.FloorToInt(hit.collider.transform.position.y)),
                        objectDrag.GetComponent<GridItemObject>().gridItem.occupiedCellsStart);
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
                        gridObj.gridItem.occupiedCellsStart))
                {
                    SetDrag(hit.collider.gameObject);
                }
            }
        }
        else
        {
            if (Time.time - timeSinceLastInput > 0.25f)
            {
                objectDrag.GetComponent<PolygonCollider2D>().enabled = false;

                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject.CompareTag("ElConttainer") &&
                    Container.Instance.CheckState(
                        new Vector2Int(Mathf.FloorToInt(hit.collider.transform.position.x), Mathf.FloorToInt(hit.collider.transform.position.y)),
                        objectDrag.GetComponent<GridItemObject>().gridItem.occupiedCellsStart))
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
    }

    /// <summary>
    /// Xử lý đầu vào từ chuột hoặc cảm ứng. Trả về tọa độ màn hình & true nếu vừa nhả.
    /// </summary>
    public bool GetInput(out Vector3 inputPos)
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager instance;

    private GameObject objectDrag;

    public void SetDrag(GameObject obj)
    {
        if(!objectDrag) objectDrag = obj;

        GridItemObject gridItemObject = objectDrag.GetComponent<GridItemObject>();
        PolygonCollider2D poly = objectDrag.GetComponent<PolygonCollider2D>();

        List<Vector2[]> paths = new List<Vector2[]>();

        foreach (Vector2 pos in gridItemObject.gridItem.occupiedCells)
        {
            Vector2[] square = new Vector2[] {
                pos + new Vector2(-0.475f, -0.475f),
                pos + new Vector2(-0.475f,  0.475f),
                pos + new Vector2( 0.475f,  0.475f),
                pos + new Vector2( 0.475f, -0.475f)
            };
            paths.Add(square);
        }

        poly.pathCount = paths.Count;
        for (int i = 0; i < paths.Count; i++)
        {
            poly.SetPath(i, paths[i]);
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            if (objectDrag != null)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (Input.GetMouseButtonUp(0))
                {
                    Container.Instance.SetState(
                        new Vector2Int(Mathf.FloorToInt(hit.collider.transform.position.x), Mathf.FloorToInt(hit.collider.transform.position.y)),
                        objectDrag.GetComponent<GridItemObject>().gridItem.occupiedCells);
                }
                objectDrag.GetComponent<PolygonCollider2D>().enabled = true;
            }
            objectDrag = null;
        }
        DragAction();
    }

    private void DragAction()
    {
        if (objectDrag == null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.tag == "ElConttainer" &&
                Container.Instance.CheckState(
                    new Vector2Int(Mathf.FloorToInt(hit.collider.transform.position.x), Mathf.FloorToInt(hit.collider.transform.position.y)),
                    objectDrag.GetComponent<GridItemObject>().gridItem.occupiedCells)
                )
            {
                if (hit.collider != null && hit.collider.gameObject.GetComponent<GridItemObject>())
                {
                    SetDrag(hit.collider.gameObject);
                }
            }
        }
        else
        {
            objectDrag.GetComponent<PolygonCollider2D>().enabled = false;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.tag == "ElConttainer" &&
                Container.Instance.CheckState(
                    new Vector2Int(Mathf.FloorToInt(hit.collider.transform.position.x), Mathf.FloorToInt(hit.collider.transform.position.y)), 
                    objectDrag.GetComponent<GridItemObject>().gridItem.occupiedCells)
                )
            {
                objectDrag.transform.position = hit.collider.transform.position;
                objectDrag.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.green;
            }
            else
            {
                objectDrag.transform.position = mousePos;
                objectDrag.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.red;
            }
        }
    }
}

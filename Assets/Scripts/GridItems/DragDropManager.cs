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
            objectDrag = null;
        }
        DragAction();
    }

    private void DragAction()
    {
        if (objectDrag == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject.GetComponent<GridItemObject>())
                {
                    SetDrag(hit.collider.gameObject);
                }
            }
        }
        else
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            objectDrag.transform.position = mousePos;
        }
    }
}

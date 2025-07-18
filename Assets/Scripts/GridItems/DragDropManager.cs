using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager instance;

    private GameObject objectDrag;

    public void SetDrag(GameObject obj)
    {
        if(!objectDrag) objectDrag = obj;
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

                if (hit.collider != null && hit.collider.transform.parent.gameObject.GetComponent<GridItemObject>())
                {
                    objectDrag = hit.collider.gameObject;
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

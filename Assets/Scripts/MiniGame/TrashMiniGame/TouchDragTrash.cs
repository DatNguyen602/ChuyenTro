using UnityEngine;
using System.Collections;
using System;

public class TouchDragTrash : MonoBehaviour
{
    [SerializeField] private Collider2D binCollider;
    private GameObject selectedObject;
    private Vector2 offset;
    private int originalOrder;
    private static int topOrder = 1000;

    public event Action OnDestroyTrash;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(touchPos, Vector2.zero);
                GameObject topTrash = null;
                int highestOrder = -999;

                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.CompareTag("Trash"))
                    {
                        SpriteRenderer sr = hit.collider.GetComponent<SpriteRenderer>();
                        if (sr != null && sr.sortingOrder > highestOrder)
                        {
                            highestOrder = sr.sortingOrder;
                            topTrash = hit.collider.gameObject;
                        }
                    }
                }

                if (topTrash != null)
                {
                    selectedObject = topTrash;

                    SpriteRenderer sr = selectedObject.GetComponent<SpriteRenderer>();
                    originalOrder = sr.sortingOrder;
                    sr.sortingOrder = topOrder;

                    offset = selectedObject.transform.position - (Vector3)touchPos;
                }
            }

            if (touch.phase == TouchPhase.Moved && selectedObject != null)
            {
                selectedObject.transform.position = touchPos + offset;
            }

            if (touch.phase == TouchPhase.Ended && selectedObject != null)
            {
                Collider2D trashCollider = selectedObject.GetComponent<Collider2D>();

                if (binCollider != null && trashCollider != null && trashCollider.bounds.Intersects(binCollider.bounds))
                {
                    StartCoroutine(ShrinkAndDestroy(selectedObject));
                    OnDestroyTrash?.Invoke();
                }
                else
                {
                    selectedObject = null;
                }
            }
        }
    }

    IEnumerator ShrinkAndDestroy(GameObject trash)
    {
        float duration = 0.3f;
        float time = 0f;
        Vector3 startScale = trash.transform.localScale;
        Vector3 startPos = trash.transform.position;
        Vector3 targetPos = binCollider.bounds.center;

        while (time < duration)
        {
            float t = time / duration;

            trash.transform.position = Vector3.Lerp(startPos, targetPos, t);

            trash.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            time += Time.deltaTime;
            yield return null;
        }

        Destroy(trash);
        selectedObject = null;
    }

}

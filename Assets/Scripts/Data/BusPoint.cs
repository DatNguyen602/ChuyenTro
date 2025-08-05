using UnityEngine;
using UnityEngine.EventSystems;

public class BusPoint : MonoBehaviour, IPointerClickHandler
{
    private bool isHover = false;
    [SerializeField] private PathFollowerDOTween pathFollower;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isHover)
        {
            MapManager.Instance.mapCanvas.SetActive(!MapManager.Instance.mapCanvas.activeSelf);
            MapManager.Instance.mapCanvas.transform.position = transform.position + Vector3.up * 0.55f;
            MapManager.Instance.onClick = pathFollower.ChangeMap;
            Debug.Log("Bus point clicked: " + gameObject.name);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isHover = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MapManager.Instance.mapCanvas.SetActive(false);
            isHover = false;
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class BusPoint : MonoBehaviour, IPointerClickHandler
{
    private bool isHover = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isHover)
        {
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
            isHover = false;
        }
    }
}

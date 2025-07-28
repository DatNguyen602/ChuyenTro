using UnityEngine;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class RoomCard : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] private TextMeshProUGUI roomInforTMP;

    private RoomInfor roomInfor;

    public event Action<RoomInfor> OnTake;
    public event Func<bool> OnSkip;
    public event Action<float> OnDragX;

    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = transform.position;
    }

    private void Start()
    {
        RandomRoom();
    }

    public void RandomRoom()
    {
        roomInfor = RoomInfor.GenerateRandomRoom();
        roomInforTMP.text = roomInfor.ToString();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        DOTween.Kill(transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position += new Vector3(eventData.delta.x, 0, 0);
        float dragX = transform.localPosition.x;

        OnDragX?.Invoke(dragX);

        float rotationZ = Mathf.Clamp(dragX / 10f, -15f, 15f);
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float dragX = transform.localPosition.x;

        if (dragX > 400f)
        {
            OnTake?.Invoke(roomInfor);
            HideCard(true);
        }
        else if (dragX < -400f)
        {
            if (OnSkip.Invoke())
            {
                HideCard(false);

            }
            else ReturnToOrigin();
        }
        else
        {
            ReturnToOrigin();
        }
        OnDragX?.Invoke(0f);

    }

    private void HideCard(bool toRight)
    {

        canvasGroup.DOFade(0f, 0.3f);
        float targetX = toRight ? Screen.width + 300 : -Screen.width - 300;

        transform.DOMoveX(targetX, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                ResetCard();
            });
    }

    private void ReturnToOrigin()
    {
        transform.DOMove(originalPosition, 0.3f).SetEase(Ease.OutBack);
        transform.DORotate(Vector3.zero, 0.3f);
    }

    private void ResetCard()
    {
        canvasGroup.alpha = 1f;
        transform.position = originalPosition;
        transform.rotation = Quaternion.identity;
    }


}

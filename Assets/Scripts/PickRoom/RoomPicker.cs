using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPicker : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private RoomCard[] roomCards;
    [SerializeField] private Image SkipNotice;
    [SerializeField] private Image TakeNotice;
    [SerializeField] private TextMeshProUGUI skipCountTMP;

    public event Action<RoomInfor> OnTake;

    private int currentIndex = 0;
    private int maxSkipCount = 5;
    private int skipCount = 5;

    private void Start()
    {
        skipCount = maxSkipCount;
        foreach (var card in roomCards)
        {
            card.gameObject.SetActive(false);
            card.OnTake += HandleTake;
            card.OnSkip += HandleSkip;
            card.OnDragX += HandleDragX;
        }

        ShowAllUI();

    }

    private void HandleDragX(float dragX)
    {
        float scale = Mathf.Clamp01(Mathf.Abs(dragX) / 200f);

        if (dragX > 0)
        {
            TakeNotice.transform.DOScale(1f + scale * 0.5f, 0.1f);
            SkipNotice.transform.DOScale(1f, 0.1f);
        }
        else
        {
            SkipNotice.transform.DOScale(1f + scale * 0.5f, 0.1f);
            TakeNotice.transform.DOScale(1f, 0.1f);
        }
    }

    private void HandleTake(RoomInfor roomInfor)
    {
        HideAllUI();
        OnTake?.Invoke(roomInfor);
        HideAllUI();
    }

    private bool HandleSkip()
    {
        if (skipCount == 0) return false;

        skipCount--;
        skipCountTMP.text = skipCount.ToString();
        if (skipCount == 0)
        {
            SkipNotice.gameObject.SetActive(false);
        }
        ShowNextCard();
        return true;
    }



    private void ShowNextCard()
    {
        currentIndex++;
        if (currentIndex >= roomCards.Length)
        {
            currentIndex = 0;
        }

        ShowCard(currentIndex);
    }

    private void ShowCard(int index)
    {
        var card = roomCards[index];
        card.RandomRoom();
        card.gameObject.SetActive(true);
        card.transform.localScale = Vector3.zero;
        card.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
    }

    private void HideAllUI()
    {
        HideElement(SkipNotice.transform, SkipNotice.gameObject);
        HideElement(TakeNotice.transform, TakeNotice.gameObject);
        HideElement(skipCountTMP.transform, skipCountTMP.gameObject);
    }

    private void ShowAllUI()
    {

        if (skipCount > 0)
        {
            ShowElement(SkipNotice.transform, SkipNotice.gameObject);
        }

        ShowElement(TakeNotice.transform, TakeNotice.gameObject);
        ShowElement(skipCountTMP.transform, skipCountTMP.gameObject);

        if (roomCards.Length > 0)
        {
            ShowCard(currentIndex);
        }
    }

    private void HideElement(Transform uiTransform, GameObject uiObject)
    {
        uiTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() => uiObject.SetActive(false));
    }

    private void ShowElement(Transform uiTransform, GameObject uiObject)
    {
        uiObject.SetActive(true);
        uiTransform.localScale = Vector3.zero;
        uiTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
}

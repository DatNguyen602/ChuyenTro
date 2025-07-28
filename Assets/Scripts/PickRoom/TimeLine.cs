using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class TimeLine : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hourText;
    [SerializeField] private TextMeshProUGUI dayText;

    private int currentHour;
    private int days;

    public event Action<int> OnTimePass;

    private void Start()
    {
        currentHour = 7;
        days = 0;
        UpdateDisplay();

    }

    public void AdvanceHour(int amount)
    {
        currentHour += amount;

        if (currentHour >= 23)
        {
            currentHour = 7;
            days++;
        }
        UpdateDisplay();
        OnTimePass?.Invoke(currentHour);
    }

    public int GetCurrentHour()
    {
        return currentHour;
    }

    public int GetCurrentDay()
    {
        return days;
    }

    private void UpdateDisplay()
    {
        if (hourText != null)
            hourText.text = $"Giờ: {currentHour}:00";
        if (dayText != null)
            dayText.text = $"Ngày: {days}";
    }
    public void ShowUI()
    {
        if (hourText != null)
        {
            hourText.gameObject.SetActive(true);

            CanvasGroup hourGroup = hourText.GetComponent<CanvasGroup>();
            if (hourGroup == null)
                hourGroup = hourText.gameObject.AddComponent<CanvasGroup>();

            hourGroup.alpha = 0f;
            hourGroup.DOFade(1f, 0.5f);
        }

        if (dayText != null)
        {
            dayText.gameObject.SetActive(true);

            CanvasGroup dayGroup = dayText.GetComponent<CanvasGroup>();
            if (dayGroup == null)
                dayGroup = dayText.gameObject.AddComponent<CanvasGroup>();

            dayGroup.alpha = 0f;
            dayGroup.DOFade(1f, 0.5f);
        }

        UpdateDisplay();
    }
    public void HideUI()
    {
        if (hourText != null)
        {
            CanvasGroup hourGroup = hourText.GetComponent<CanvasGroup>();
            if (hourGroup == null)
                hourGroup = hourText.gameObject.AddComponent<CanvasGroup>();

            hourGroup.DOFade(0f, 0.5f).OnComplete(() =>
            {
                hourText.gameObject.SetActive(false);
            });
        }

        if (dayText != null)
        {
            CanvasGroup dayGroup = dayText.GetComponent<CanvasGroup>();
            if (dayGroup == null)
                dayGroup = dayText.gameObject.AddComponent<CanvasGroup>();

            dayGroup.DOFade(0f, 0.5f).OnComplete(() =>
            {
                dayText.gameObject.SetActive(false);
            });
        }
    }


}

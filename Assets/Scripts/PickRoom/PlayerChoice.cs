using System;
using TMPro;
using UnityEngine;

public class PlayerChoice : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI desTMP;

    public string label;
    public string consequenceText;
    public int timeCost;
    public int startTime;
    public int endTime;

    public TimeLine timeLine;


    public Action OnApply;
    public Action OnIsValid;
    public Action OnExpired;


    public void SetDescription()
    {
        if (desTMP != null)
        {
            desTMP.text = $"{label}\n{consequenceText}";
        }
    }

    public void SetTimeLine(TimeLine timeLine)
    {
        this.timeLine = timeLine;
        this.timeLine.OnTimePass += OnTimePass;
    }

    private void OnTimePass(int time)
    {
        OnIsValid?.Invoke();
        if (time == endTime)
        {
            OnExpired?.Invoke();
            this.timeLine.OnTimePass -= OnTimePass;
            Destroy(this.gameObject);
        }

    }


    public void Apply()
    {
        if(startTime<timeLine.GetCurrentHour()) return;
        timeLine.AdvanceHour(timeCost);
        OnApply?.Invoke();
        this.timeLine.OnTimePass -= OnTimePass;
        Destroy(this.gameObject);
    }

}

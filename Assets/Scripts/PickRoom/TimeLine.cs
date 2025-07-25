using System;
using UnityEngine;

public class TimeLine : MonoBehaviour
{
    private int currentHour;
    private int days;

    public event Action<int> OnTimePass;

    public void AdvanceHour(int amount)
    {
        currentHour += amount;

        if (currentHour >= 23)
        {
            currentHour = 7;
            days++;
        }
        OnTimePass?.Invoke(currentHour);
    }

    public int GetCurrentHour()
        { return currentHour; }

}

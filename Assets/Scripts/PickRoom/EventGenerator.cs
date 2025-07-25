

using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class EventGenerator : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private Transform sctrollView;
    [SerializeField] private Transform choicesParent;
    [SerializeField] private PlayerChoice choicePrefab;

    public void GenEvent(PlayerStats playerStats, RoomInfor roomInfor, TimeLine timeLine)
    {


        if (playerStats.hunger > 40f)
        {
            var choiceGO = Instantiate(choicePrefab, choicesParent);
            var choice = choiceGO.GetComponent<PlayerChoice>();

            choice.label = "đi ăn";
            choice.consequenceText = "Bạn ăn món bình dân ở phòng trọ.";
            choice.timeCost = 1;
            choice.SetTimeLine(timeLine);

            choice.OnApply = () =>
            {
                playerStats.hunger = Mathf.Clamp(playerStats.hunger - 40f, 0f, 100f);
                playerStats.money -= 30f;
                playerStats.stress = Mathf.Clamp(playerStats.stress - 5f, 0f, 100f);
            };

            choice.SetDescription();
        }

        if (playerStats.sleepiness > 50f)
        {
            var choiceGO = Instantiate(choicePrefab, choicesParent);
            var choice = choiceGO.GetComponent<PlayerChoice>();

            choice.label = "ngủ";
            choice.consequenceText = "Bạn nghỉ ngơi tại phòng.";
            choice.timeCost = 2;
            choice.SetTimeLine(timeLine);


            choice.OnApply = () =>
            {
                playerStats.sleepiness = Mathf.Clamp(playerStats.sleepiness - 35f, 0f, 100f);
                playerStats.stress = Mathf.Clamp(playerStats.stress - 10f, 0f, 100f);
                playerStats.hunger = Mathf.Clamp(playerStats.hunger + 10f, 0f, 100f);
            };

            choice.SetDescription();
        }

    }

    public void ShowChoices()
    {
        sctrollView.gameObject.SetActive(true);
        RectTransform rt = sctrollView.GetComponent<RectTransform>();
        Vector2 targetPos = rt.anchoredPosition; 
        rt.anchoredPosition = targetPos - new Vector2(0, 300); 

        rt.DOAnchorPos(targetPos, 0.5f).SetEase(Ease.OutCubic);

        CanvasGroup group = sctrollView.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 0f;
            group.DOFade(1f, 0.5f);
        }
    }    


}

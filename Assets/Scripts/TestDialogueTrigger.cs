using UnityEngine;
using System.Collections.Generic;

public class TestDialogueTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;

    void Start()
    {
        List<DialogueChoiceData> choices = new List<DialogueChoiceData>();

        choices.Add(new DialogueChoiceData
        {
            choiceText = "Sửa ổ điện (-200k, -1 stress)",
            moneyChange = -200000,
            stressChange = -1,
            emotionChange = 0,
            resultText = "Ổ điện sáng trở lại."
        });

        choices.Add(new DialogueChoiceData
        {
            choiceText = "Phớt lờ (+1 stress)",
            moneyChange = 0,
            stressChange = 1,
            emotionChange = 0,
            resultText = "Cả tối hôm đó không học được."
        });

        choices.Add(new DialogueChoiceData
        {
            choiceText = "Nhờ bạn sửa (+100k, -2 stress)",
            moneyChange = -100000,
            stressChange = -2,
            emotionChange = 0,
            resultText = "Bạn giúp nhiệt tình, mọi thứ ổn."
        });

        dialogueManager.ShowDialogue("Bạn cùng trọ", "Ổ điện hỏng rồi, sửa không?", choices);
    }
}

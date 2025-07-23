using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueLineText;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;

    public int playerMoney = 3000000;
    public int playerStress = 3;
    public int playerEmotion = 5;

    public void ShowDialogue(string npcName, string dialogueText, List<DialogueChoiceData> choices)
    {
        dialoguePanel.SetActive(true);
        npcNameText.text = npcName;
        dialogueLineText.text = dialogueText;

        // Xóa các nút cũ
        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        // Tạo các lựa chọn mới
        foreach (var choice in choices)
        {
            GameObject btn = Instantiate(choiceButtonPrefab, choiceContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                ApplyChoice(choice);
                dialoguePanel.SetActive(false);
            });
        }
    }

    void ApplyChoice(DialogueChoiceData choice)
    {
        playerMoney += choice.moneyChange;
        playerStress += choice.stressChange;
        playerEmotion += choice.emotionChange;

        // Log ra console để kiểm tra
        Debug.Log($"Money: {playerMoney} | Stress: {playerStress} | Emotion: {playerEmotion}");

        // TODO: Hiển thị text phản hồi nếu muốn
        if (!string.IsNullOrEmpty(choice.resultText))
        {
            Debug.Log("Result: " + choice.resultText);
        }

        // Gọi sự kiện nếu có
        choice.onSelectEvent?.Invoke();
    }
}

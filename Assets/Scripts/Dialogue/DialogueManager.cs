using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueLineText;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;

    public void ShowDialogue(string npcName,
                             string dialogueText,
                             List<DialogueChoiceData> choices)
    {
        dialoguePanel.SetActive(true);
        npcNameText.text = npcName;
        dialogueLineText.text = dialogueText;

        // Xoá các nút cũ
        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        // Tạo nút mới cho từng choice
        foreach (DialogueChoiceData choice in choices)
        {
            GameObject btn = Instantiate(choiceButtonPrefab, choiceContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            // ghi riêng biến local để tránh vấn đề capture foreach
            DialogueChoiceData captured = choice;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                ApplyChoice(captured);
                dialoguePanel.SetActive(false);
            });
        }
    }

    void ApplyChoice(DialogueChoiceData choice)
    {
        // Lấy singleton quản lý stat
        var gm = GamePlayManager.instance;

        gm.Money += choice.moneyChange;   // tự clamp ≥ 0 trong property
        gm.Stress += choice.stressChange;  // clamp 0–100
        gm.Emotion += choice.emotionChange; // clamp 0–10

        Debug.Log($"Money: {gm.Money} | Stress: {gm.Stress} | Emotion: {gm.Emotion}");

        // Hiển thị result text (nếu có) hoặc gọi popup riêng
        if (!string.IsNullOrEmpty(choice.resultText))
            Debug.Log("Result: " + choice.resultText);

        // Gọi UnityEvent tuỳ chỉnh nếu designer gắn
        choice.onSelectEvent?.Invoke();

        // (tuỳ chọn) cập‑nhật HUD nếu bạn có StatUIController
        // FindObjectOfType<StatUIController>()?.UpdateStats(gm.Money, gm.Emotion);
    }
}

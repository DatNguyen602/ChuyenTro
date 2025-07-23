using UnityEngine;

[System.Serializable]
public class DialogueChoiceData
{
    public string choiceText;       // Nội dung hiển thị trên nút
    public int moneyChange;         // + hoặc - tiền
    public int stressChange;        // + hoặc -
    public int emotionChange;       // + hoặc -
    public string resultText;       // Text hiển thị sau khi chọn (nếu muốn)

    public UnityEngine.Events.UnityEvent onSelectEvent; // sự kiện tùy chỉnh (nâng cao)
}

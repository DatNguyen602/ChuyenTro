using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameSelectionElement : MonoBehaviour
{
    [SerializeField] private Image gameImage;
    [SerializeField] private TextMeshProUGUI gameNameTMP;
    [SerializeField] private TextMeshProUGUI costTMP;
    [SerializeField] private TextMeshProUGUI rewardTMP;
    [SerializeField] private Button playBtn;
    
    private MiniGameData gameData;
    
   public void SetData(MiniGameData gameData)
    {
        this.gameData = gameData;
        gameImage.sprite = gameData.icon;
        gameNameTMP.text = gameData.name;
        costTMP.text = gameData.costToPlay.ToString();
        rewardTMP.text = gameData.rewardOnWin.ToString();
    }    
}

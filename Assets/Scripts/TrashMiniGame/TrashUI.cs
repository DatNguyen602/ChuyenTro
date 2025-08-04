using TMPro;
using UnityEngine;

public class TrashUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI timeTMP;
    [SerializeField] private GameObject doneTxt;
    [SerializeField] private GameObject timeOutTxt;
    
    public void SetTimeText(int time)
    {
        timeTMP.text = time.ToString();
    }  

    public void ShowDoneVisual()
    {
        doneTxt.SetActive(true);
    }    

    public void ShowTimeOutVisual()
    {
        timeOutTxt.SetActive(true);
    }    
        
}

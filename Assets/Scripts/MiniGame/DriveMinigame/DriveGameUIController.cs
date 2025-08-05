using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DriveGameUIController : MonoBehaviour
{
    [SerializeField] private Button playBtn;
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button replayBtn;

    [SerializeField] private TextMeshProUGUI timeTMP;

    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;


    private void Start()
    {
        timeTMP.text = DriveGameManager.Instance.GameDuration.ToString("F2");
        playBtn.onClick.AddListener(() => DriveGameManager.Instance.Play());
        //pauseBtn.onClick.AddListener(() => DriveGameManager.Instance.Pause());
        //resumeBtn.onClick.AddListener(() => DriveGameManager.Instance.Resume());
        //replayBtn.onClick.AddListener(() => DriveGameManager.Instance.Replay());

        DriveGameManager.Instance.OnPlay += () =>
        {
            playBtn.gameObject.SetActive(false);
            //pauseBtn.gameObject.SetActive(true);
            //resumeBtn.gameObject.SetActive(false);
            //replayBtn.gameObject.SetActive(false);
        };

        /*DriveGameManager.Instance.OnPause += () =>
        {
            pauseBtn.gameObject.SetActive(false);
            resumeBtn.gameObject.SetActive(true);
        };

        DriveGameManager.Instance.OnResume += () =>
        {
            pauseBtn.gameObject.SetActive(true);
            resumeBtn.gameObject.SetActive(false);
        };*/

        DriveGameManager.Instance.OnLose += () =>
        {
            losePanel.gameObject.SetActive(true);
            //replayBtn.gameObject.SetActive(true);
            //pauseBtn.gameObject.SetActive(false);
            //resumeBtn.gameObject.SetActive(false);
        };
        /* DriveGameManager.Instance.OnReplay += () =>
         {
             replayBtn.gameObject.SetActive(false);
             playBtn.gameObject.SetActive(true);
         };*/
        DriveGameManager.Instance.OnTimeUpdate += (float timer) =>
        {
            timeTMP.text = timer.ToString("F2");
        };
        DriveGameManager.Instance.OnWin += () =>
        {
            winPanel.gameObject.SetActive(true);
           
        };

    }
}

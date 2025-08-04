using UnityEngine;
using UnityEngine.UI;

public class DriveGameUIController : MonoBehaviour
{
    [SerializeField] private Button playBtn;
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button replayBtn;

    private void Start()
    {
        playBtn.onClick.AddListener(() => DriveGameManager.Instance.Play());
        pauseBtn.onClick.AddListener(() => DriveGameManager.Instance.Pause());
        resumeBtn.onClick.AddListener(() => DriveGameManager.Instance.Resume());
        replayBtn.onClick.AddListener(() => DriveGameManager.Instance.Replay());

        DriveGameManager.Instance.OnPlay += () =>
        {
            playBtn.gameObject.SetActive(false);
            pauseBtn.gameObject.SetActive(true);
            resumeBtn.gameObject.SetActive(false);
            replayBtn.gameObject.SetActive(false);
        };

        DriveGameManager.Instance.OnPause += () =>
        {
            pauseBtn.gameObject.SetActive(false);
            resumeBtn.gameObject.SetActive(true);
        };

        DriveGameManager.Instance.OnResume += () =>
        {
            pauseBtn.gameObject.SetActive(true);
            resumeBtn.gameObject.SetActive(false);
        };

        DriveGameManager.Instance.OnLose += () =>
        {
            replayBtn.gameObject.SetActive(true);
            pauseBtn.gameObject.SetActive(false);
            resumeBtn.gameObject.SetActive(false);
        };
        DriveGameManager.Instance.OnReplay += () =>
        {
            replayBtn.gameObject.SetActive(false);
            playBtn.gameObject.SetActive(true);
        };


    }
}

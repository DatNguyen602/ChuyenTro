using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiniGameSelector : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private Button driveGameBtn;
    [SerializeField] private Button dartGameButton;
    [SerializeField] private Button cleanTrashGameButton;

    void Start()
    {
        driveGameBtn.onClick.AddListener(() => LoadScene("DriveMiniGame"));
        dartGameButton.onClick.AddListener(() => LoadScene("DartMiniGame"));
        cleanTrashGameButton.onClick.AddListener(() => LoadScene("TrashMiniGame"));
    }

    private void LoadScene(string sceneName)
    {
        TempRoomData.isSetPos = true;
        TempRoomData.userPosition = GamePlayManager.instance.Player.transform.position;
        SceneManager.LoadScene(sceneName);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            container.SetActive(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            container.SetActive(false);
        }
    }


}

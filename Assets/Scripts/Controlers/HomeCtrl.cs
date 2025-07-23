using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeCtrl : MonoBehaviour
{
    [SerializeField] private Transform boardTransform, btnCtrls;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private GameObject LoadingUI;

    public void CloseAllBoard()
    {
        if(boardTransform)
        {
            foreach (Transform child in boardTransform)
            {
                if (child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
        if(btnCtrls)
        {
            foreach (Transform child in btnCtrls)
            {
                if (child.gameObject.activeSelf)
                {
                    child.GetComponent<Image>().color = Color.white;
                }
            }
        }
    }

    public void SetSelectBtn(Image img)
    {
        if(img)
        {
            img.color = selectedColor;
        }
    }

    private float timer = 0.0f;
    public void LoadingScene(string sceneName)
    {
        timer = 5.0f;
        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        if (LoadingUI) LoadingUI.SetActive(true);
        if (scene != null)
        {
            scene.allowSceneActivation = false;
            while (!scene.isDone)
            {
                if (scene.progress >= 0.9f && timer <= 0)
                {
                    scene.allowSceneActivation = true;
                    //if (LoadingUI) LoadingUI.SetActive(false);
                    return;
                }
                else
                {
                    timer -= Time.deltaTime;
                }
            }
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

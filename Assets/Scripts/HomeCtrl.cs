using UnityEngine;
using UnityEngine.UI;

public class HomeCtrl : MonoBehaviour
{
    [SerializeField] private Transform boardTransform, btnCtrls;
    [SerializeField] private Color selectedColor = Color.green;

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

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

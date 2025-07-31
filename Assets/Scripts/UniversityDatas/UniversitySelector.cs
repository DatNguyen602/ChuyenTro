using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UniversitySelector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI universityNameTMP;
    [SerializeField] private SpriteRenderer logoSpriteRenderer;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button okButton;

    private UniversityData[] universityDatas;
    private int currentIndex = 0;

    void Start()
    {
        universityDatas = Resources.LoadAll<UniversityData>("UniversityDatas");

        if (universityDatas.Length > 0)
        {
            ShowUniversity(currentIndex);
        }

        nextButton.onClick.AddListener(ShowNextUniversity);
        previousButton.onClick.AddListener(ShowPreviousUniversity);
        okButton.onClick.AddListener(OnOkClicked);
    }
    private void OnOkClicked()
    {
        GameData.Instance.selectedLogo = universityDatas[currentIndex].logo;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Map"); // chuyển scene
    }

    public void ShowNextUniversity()
    {
        if (universityDatas.Length == 0) return;
        currentIndex = (currentIndex + 1) % universityDatas.Length;
        ShowUniversity(currentIndex);
    }

    public void ShowPreviousUniversity()
    {
        if (universityDatas.Length == 0) return;
        currentIndex = (currentIndex - 1 + universityDatas.Length) % universityDatas.Length;
        ShowUniversity(currentIndex);
    }

    private void ShowUniversity(int index)
    {
        UniversityData data = universityDatas[index];
        universityNameTMP.text = data.universityName;
        logoSpriteRenderer.sprite = data.logo;
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoPanel : MonoBehaviour
{
    public static ItemInfoPanel instance;
    public Image itemImage;
    public TextMeshProUGUI nameText, priceText, brokenText;
    public Button actionButton;
    private GridItem currentItem;
    private bool isShopMode;

    private Vector2 startSizeImg;

    void Awake() => instance = this;

    private void Start()
    {
        startSizeImg = itemImage.GetComponent<RectTransform>().sizeDelta;
    }

    public void Show(GridItem item, bool shopMode)
    {
        currentItem = item;
        isShopMode = shopMode;
        itemImage.sprite = item.sprite;
        RectTransform rectTransform = itemImage.GetComponent<RectTransform>();
        if (item.sprite.rect.height < item.sprite.rect.width)
        {
            rectTransform.sizeDelta = new Vector2(
                startSizeImg.x,
                (startSizeImg.x * item.sprite.rect.height) / item.sprite.rect.width
                );
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(
                (startSizeImg.y * item.sprite.rect.width) / item.sprite.rect.height,
                startSizeImg.x
                );
        }
            nameText.text = item.itemName;
        priceText.text = "Giá: " + item.price;
        brokenText.text = "Tỉ lệ hỏng: " + (item.brokenRate * 100f) + "%";
        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = shopMode ? "Mua" : "Bán";
        actionButton.onClick.RemoveAllListeners();
        if (shopMode)
            actionButton.onClick.AddListener(() => BuyAndSell.instance.BuyItem(item));
        else
            actionButton.onClick.AddListener(() => BuyAndSell.instance.SellItem(item));
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentItem = null;
    }
}
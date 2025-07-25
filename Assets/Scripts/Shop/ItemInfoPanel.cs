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

    void Awake() => instance = this;

    public void Show(GridItem item, bool shopMode)
    {
        currentItem = item;
        isShopMode = shopMode;
        itemImage.sprite = item.sprite;
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
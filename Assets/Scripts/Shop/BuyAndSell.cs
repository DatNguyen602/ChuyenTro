using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyAndSell : MonoBehaviour
{
    public static BuyAndSell instance;
    [SerializeField] public List<GridItem> itemsToSell = new List<GridItem>();
    [SerializeField] private Transform ParentSell;
    [SerializeField] private Transform ParentBuy;
    [SerializeField] private GameObject infoPanelObj; // Kéo InfoPanel vào đây

    private Button lastSelectedButton = null;

    void Awake() => instance = this;

    void Start()
    {
        RenderSellList();
        RenderBuyList();
        ItemInfoPanel.instance.Hide();
    }

    public void RenderSellList()
    {
        for (int i = ParentSell.childCount - 1; i >= 0; i--)
            Destroy(ParentSell.GetChild(i).gameObject);

        foreach (var i in itemsToSell)
        {
            GameObject temp = GamePlayManager.instance.RendererList(i, ParentSell, null);
            Button selectButton = temp.GetComponent<Button>();
            if (selectButton == null) selectButton = temp.AddComponent<Button>();
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => OnShopItemClick(i, selectButton));
        }
    }

    private void OnShopItemClick(GridItem item, Button btn)
    {
        if (lastSelectedButton == btn)
        {
            btn.image.color = Color.white;
            lastSelectedButton = null;
            ItemInfoPanel.instance.Hide();
            // Đẩy ParentSell lên (ẩn panel info)
            // ParentSell.GetComponent<RectTransform>().anchoredPosition = new Vector2(...);
            return;
        }
        if (lastSelectedButton != null)
            lastSelectedButton.image.color = Color.white;
        btn.image.color = Color.yellow;
        lastSelectedButton = btn;
        ItemInfoPanel.instance.Show(item, true);
        // Hạ ParentSell xuống (hiện panel info)
        // ParentSell.GetComponent<RectTransform>().anchoredPosition = new Vector2(...);
    }

    public void BuyItem(GridItem item)
    {
        if (GamePlayManager.instance.Money < item.price)
        {
            // Thông báo không đủ tiền
            return;
        }
        GamePlayManager.instance.Money -= item.price;
        GridManager.instance.gridItems.Add(item);
        RenderBuyList();
        GridManager.instance.RendererList();
        ItemInfoPanel.instance.Hide();
        if (lastSelectedButton != null) lastSelectedButton.image.color = Color.white;
        lastSelectedButton = null;
        // Đẩy ParentSell lên
    }

    public void RenderBuyList()
    {
        for (int i = ParentBuy.childCount - 1; i >= 0; i--)
            Destroy(ParentBuy.GetChild(i).gameObject);

        foreach (var i in GridManager.instance.gridItems)
        {
            GameObject temp = GamePlayManager.instance.RendererList(i, ParentBuy, null);
            Button selectButton = temp.GetComponent<Button>();
            if (selectButton == null) selectButton = temp.AddComponent<Button>();
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => OnBuyItemClick(i, selectButton));
        }
    }

    private void OnBuyItemClick(GridItem item, Button btn)
    {
        if (lastSelectedButton == btn)
        {
            btn.image.color = Color.white;
            lastSelectedButton = null;
            ItemInfoPanel.instance.Hide();
            // Đẩy ParentBuy lên
            return;
        }
        if (lastSelectedButton != null)
            lastSelectedButton.image.color = Color.white;
        btn.image.color = Color.yellow;
        lastSelectedButton = btn;
        ItemInfoPanel.instance.Show(item, false);
        // Hạ ParentBuy xuống
    }

    public void SellItem(GridItem item)
    {
        GamePlayManager.instance.Money += item.price;
        GridManager.instance.gridItems.Remove(item);
        RenderBuyList();
        GridManager.instance.RendererList();
        ItemInfoPanel.instance.Hide();
        if (lastSelectedButton != null) lastSelectedButton.image.color = Color.white;
        lastSelectedButton = null;
        // Đẩy ParentBuy lên
    }
}
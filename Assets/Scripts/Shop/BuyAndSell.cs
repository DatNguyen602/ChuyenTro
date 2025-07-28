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
    [SerializeField] private RectTransform ControlSell;
    [SerializeField] private RectTransform ControlBuy;
    [SerializeField] private RectTransform ImageSell;
    [SerializeField] private RectTransform ImageBuy;
    private Button lastSelectedButton = null;

    // Lưu lại toàn bộ thông số gốc của RectTransform
    private Vector2 controlSellOriginOffsetMin;
    private Vector2 controlSellOriginOffsetMax;
    private Vector2 controlSellOriginAnchorMin;
    private Vector2 controlSellOriginAnchorMax;
    private Vector2 controlSellOriginPivot;

    private Vector2 controlBuyOriginOffsetMin;
    private Vector2 controlBuyOriginOffsetMax;
    private Vector2 controlBuyOriginAnchorMin;
    private Vector2 controlBuyOriginAnchorMax;
    private Vector2 controlBuyOriginPivot;

    // Thông số target khi hiện info (top stretch - top, pivot center)
    private readonly float infoHeight = 1162f;
    private readonly float infoPosY = -1084f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Lưu lại thông số gốc của RectTransform
        controlSellOriginOffsetMin = ControlSell.offsetMin;
        controlSellOriginOffsetMax = ControlSell.offsetMax;
        controlSellOriginAnchorMin = ControlSell.anchorMin;
        controlSellOriginAnchorMax = ControlSell.anchorMax;
        controlSellOriginPivot = ControlSell.pivot;

        controlBuyOriginOffsetMin = ControlBuy.offsetMin;
        controlBuyOriginOffsetMax = ControlBuy.offsetMax;
        controlBuyOriginAnchorMin = ControlBuy.anchorMin;
        controlBuyOriginAnchorMax = ControlBuy.anchorMax;
        controlBuyOriginPivot = ControlBuy.pivot;

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
            // Reset RectTransform về gốc
            ResetRectTransform(ControlSell,
                controlSellOriginOffsetMin,
                controlSellOriginOffsetMax,
                controlSellOriginAnchorMin,
                controlSellOriginAnchorMax,
                controlSellOriginPivot);
            return;
        }
        if (lastSelectedButton != null)
            lastSelectedButton.image.color = Color.white;
        btn.image.color = Color.yellow;
        lastSelectedButton = btn;
        ItemInfoPanel.instance.Show(item, true);
        // Đặt RectTransform về thông số info
        SetRectTransformForInfo(ControlSell);
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
        // Reset RectTransform về gốc
        ResetRectTransform(ControlSell,
            controlSellOriginOffsetMin,
            controlSellOriginOffsetMax,
            controlSellOriginAnchorMin,
            controlSellOriginAnchorMax,
            controlSellOriginPivot);
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
            // Reset RectTransform về gốc
            ResetRectTransform(ControlBuy,
                controlBuyOriginOffsetMin,
                controlBuyOriginOffsetMax,
                controlBuyOriginAnchorMin,
                controlBuyOriginAnchorMax,
                controlBuyOriginPivot);
            return;
        }
        if (lastSelectedButton != null)
            lastSelectedButton.image.color = Color.white;
        btn.image.color = Color.yellow;
        lastSelectedButton = btn;
        ItemInfoPanel.instance.Show(item, false);
        // Đặt RectTransform về thông số info
        SetRectTransformForInfo(ControlBuy);
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
        // Reset RectTransform về gốc
        ResetRectTransform(ControlBuy,
            controlBuyOriginOffsetMin,
            controlBuyOriginOffsetMax,
            controlBuyOriginAnchorMin,
            controlBuyOriginAnchorMax,
            controlBuyOriginPivot);
    }

    // Gọi hàm này khi chuyển tab Shop/ListItem để reset UI
    public void OnTabChanged()
    {
        ItemInfoPanel.instance.Hide();
        if (lastSelectedButton != null)
        {
            lastSelectedButton.image.color = Color.white;
            lastSelectedButton = null;
        }
        // Reset vị trí panel
        ResetRectTransform(ControlSell,
            controlSellOriginOffsetMin,
            controlSellOriginOffsetMax,
            controlSellOriginAnchorMin,
            controlSellOriginAnchorMax,
            controlSellOriginPivot);
        ResetRectTransform(ControlBuy,
            controlBuyOriginOffsetMin,
            controlBuyOriginOffsetMax,
            controlBuyOriginAnchorMin,
            controlBuyOriginAnchorMax,
            controlBuyOriginPivot);
    }

    // Hàm đặt lại RectTransform về gốc
    private void ResetRectTransform(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
    {
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
    }

    // Hàm đặt RectTransform về thông số info (top stretch - top, pivot center)
    private void SetRectTransformForInfo(RectTransform rt)
    {
        // Anchor stretch top
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        // Pivot center
        rt.pivot = new Vector2(0.5f, 0.5f);

        // Đặt lại chiều cao và vị trí
        float height = infoHeight;
        float posY = infoPosY;

        // offsetMax = (0, posY + height/2), offsetMin = (0, posY - height/2)
        rt.offsetMax = new Vector2(0f, posY + height / 2f);
        rt.offsetMin = new Vector2(0f, posY - height / 2f);

        // localPosition.z = 0 (nếu cần)
        var lp = rt.localPosition;
        rt.localPosition = new Vector3(lp.x, lp.y, 0f);
    }
    // Hàm bật tab Shop
    public void ShowShopPanel()
    {
        ControlSell.gameObject.SetActive(true);
        ControlBuy.gameObject.SetActive(false);
        ImageSell.gameObject.SetActive(true);
        ImageBuy.gameObject.SetActive(false);
        // Reset vị trí panel
        ResetRectTransform(ControlSell,
            controlSellOriginOffsetMin,
            controlSellOriginOffsetMax,
            controlSellOriginAnchorMin,
            controlSellOriginAnchorMax,
            controlSellOriginPivot);

        // Ẩn InfoPanel bằng hệ thống quản lý
        ItemInfoPanel.instance.Hide();

        // Reset trạng thái button nếu có
        if (lastSelectedButton != null)
        {
            lastSelectedButton.image.color = Color.white;
            lastSelectedButton = null;
        }
    }

    // Hàm bật tab List Item
    public void ShowListPanel()
    {
        ControlSell.gameObject.SetActive(false);
        ControlBuy.gameObject.SetActive(true);
        ImageSell.gameObject.SetActive(false);
        ImageBuy.gameObject.SetActive(true);
        // Reset vị trí panel
        ResetRectTransform(ControlBuy,
            controlBuyOriginOffsetMin,
            controlBuyOriginOffsetMax,
            controlBuyOriginAnchorMin,
            controlBuyOriginAnchorMax,
            controlBuyOriginPivot);

        // Ẩn InfoPanel bằng hệ thống quản lý
        ItemInfoPanel.instance.Hide();

        // Reset trạng thái button nếu có
        if (lastSelectedButton != null)
        {
            lastSelectedButton.image.color = Color.white;
            lastSelectedButton = null;
        }
    }

}
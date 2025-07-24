using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class BuyAndSell : MonoBehaviour
{
    [SerializeField] private List<GridItem> itemsToSell = new List<GridItem>();
    [SerializeField] private List<GridItem> itemsToBuy = new List<GridItem>();
    [SerializeField] private Transform ParentSell;
    [SerializeField] private Transform ParentBuy;
    void Start()
    {
        for (int i = 0; i < itemsToSell.Count; i++)
        {
            GamePlayManager.instance.RendererList(itemsToSell[i], ParentSell, i < ParentSell.childCount ? ParentSell.GetChild(i).gameObject : null);
        }
    }
}

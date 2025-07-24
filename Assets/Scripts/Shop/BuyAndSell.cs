using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class BuyAndSell : MonoBehaviour
{
    [SerializeField] private List<GridItem> itemsToSell = new List<GridItem>();
    [SerializeField] private List<GridItem> itemsToBuy = new List<GridItem>();
    [SerializeField] private Transform ParentSell;
    [SerializeField] private Transform ParentBuy;
    void Start()
    {
        foreach(var i in itemsToSell)//for (int i = 0; i < itemsToSell.Count; i++)
        {
            GameObject temp = GamePlayManager.instance.RendererList(i, ParentSell, itemsToSell.IndexOf(i) < ParentSell.childCount ? ParentSell.GetChild(itemsToSell.IndexOf(i)).gameObject : null);

            Button SelectButton = temp.GetComponent<Button>() ?? temp.AddComponent<Button>();
            SelectButton.onClick.RemoveAllListeners();
            SelectButton.onClick.AddListener(() =>
            {
                Debug.Log(i.id);
                Debug.Log(i.price);
            });
        }
    }
}

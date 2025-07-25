using UnityEngine;

public class HexTileVisual : MonoBehaviour
{
    [SerializeField] private GameObject hightlightGOj;
   
    public void SetSelectHighlight(bool active)
    {
        hightlightGOj.SetActive(active);
    }    
}
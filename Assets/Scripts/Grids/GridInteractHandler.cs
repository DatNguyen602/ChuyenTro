using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridInteractHandler : MonoBehaviour
{
    [SerializeField] private GameObject playerVisual;

    [SerializeField] private HexTile currentTile;
    private HexTile selectedTile;

    private List<HexTile> pathTiles = new();
    HexTile lastTile = null;



    private void Start()
    {
        DetectCurrentTileFromPlayer();
    }


   

    public void TileInteract()
    {
        if (!InputReader.Instance.TapDetected)
            return ;

        if (UIManager.Instance.IsUIBlock()) return;

        Vector2 screenPos = InputReader.Instance.currentTouchPos;
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        UIManager.Instance.HideAllUI();

        if (hit.collider != null)
        {
           
            HexTile tile = hit.collider.GetComponent<HexTile>();
            if (tile != null )
            {
                tile.Interact();
                selectedTile = tile;
                if( selectedTile.Isneighbor(currentTile) )
                {
                    UIManager.Instance.MoveUI.ShowUI();
                }    
                
            }
        }

    }

    public void MoveToSelectedTile()
    {
        if (currentTile != null)
        {
            List<HexTile> neighbors = currentTile.GetNeighbors();
            foreach (HexTile neighbor in neighbors)
            {
                neighbor.HexTileVisual.SetSelectHighlight(false);
            }

        }
        currentTile = selectedTile;
        if (currentTile != null)
        {
            List<HexTile> neighbors = currentTile.GetNeighbors();
            foreach (HexTile neighbor in neighbors)
            {
                neighbor.HexTileVisual.SetSelectHighlight(true);
            }

        }
        StartCoroutine(MoveToTileCoroutine(selectedTile.transform.position));
    }    
    private IEnumerator MoveToTileCoroutine(Vector2 tilePos)
    {
        float duration = 0.1f; 
        float elapsed = 0f;

        Vector3 startPos = playerVisual.transform.position;
        Vector3 targetPos = new Vector3(tilePos.x, tilePos.y, transform.position.z);

        while (elapsed < duration)
        {
            playerVisual.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerVisual.transform.position = targetPos;
    }    


    private void DetectCurrentTileFromPlayer()
    {
        Vector2 origin = playerVisual.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero);

        if (hit.collider != null)
        {
            HexTile tile = hit.collider.GetComponent<HexTile>();
            if (tile != null)
                currentTile = tile;
        }
        if (currentTile!=null)
        {
            List<HexTile> neighbors = currentTile.GetNeighbors();
            foreach (HexTile neighbor in neighbors)
            {
                neighbor.HexTileVisual.SetSelectHighlight(true);
            }

        }
    }
}

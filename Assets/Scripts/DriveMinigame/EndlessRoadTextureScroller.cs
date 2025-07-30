using UnityEngine;

public class EndlessRoadTextureScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.1f; 
    [SerializeField] private Renderer roadRenderer;  

    private Vector2 currentOffset = Vector2.zero;

    void Update()
    {
        currentOffset.y += scrollSpeed * Time.deltaTime;

        roadRenderer.material.mainTextureOffset = currentOffset;
    }
}

using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator2D : MonoBehaviour
{
    [Header("Sprites & Timing")]
    [Tooltip("Danh sách sprite sẽ animate qua lại")]
    public List<Sprite> sprites = new List<Sprite>();

    [Tooltip("Thời gian hiển thị mỗi sprite (giây)")]
    public float frameDuration = 0.1f;

    [Header("Loop Settings")]
    [Tooltip("Nếu true: sau sprite cuối, sẽ quay ngược về đầu")]
    public bool pingPong = true;

    [Tooltip("Số lần lặp. -1 = loop vô hạn")]
    public int loops = -1;

    private SpriteRenderer _renderer;
    private Sequence _sequence;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        PlayAnimation();
    }

    void OnDisable()
    {
        StopAnimation();
    }

    /// <summary>
    /// Xây dựng và chơi sequence DOTween
    /// </summary>
    [ContextMenu("Play Animation")]
    public void PlayAnimation()
    {
        if (sprites == null || sprites.Count == 0)
        {
            Debug.LogWarning("SpriteAnimator2D: Chưa có sprite nào để animate.");
            return;
        }

        StopAnimation();

        _sequence = DOTween.Sequence();
        int count = sprites.Count;

        // 1. Forward: 0 -> last
        for (int i = 0; i < count; i++)
        {
            int idx = i;
            _sequence.AppendCallback(() => _renderer.sprite = sprites[idx]);
            _sequence.AppendInterval(frameDuration);
        }

        // 2. PingPong: last-1 -> 1
        if (pingPong && count > 2)
        {
            for (int i = count - 2; i > 0; i--)
            {
                int idx = i;
                _sequence.AppendCallback(() => _renderer.sprite = sprites[idx]);
                _sequence.AppendInterval(frameDuration);
            }
        }

        // 3. Loop
        _sequence.SetLoops(loops, LoopType.Restart);
        _sequence.Play();
    }

    /// <summary>
    /// Dừng animation và kill sequence
    /// </summary>
    [ContextMenu("Stop Animation")]
    public void StopAnimation()
    {
        if (_sequence != null && _sequence.IsActive())
        {
            _sequence.Kill();
            _sequence = null;
        }
    }

    /// <summary>
    /// Cập nhật trực tiếp list sprite (ví dụ runtime), và restart animation
    /// </summary>
    public void UpdateSprites(List<Sprite> newSprites)
    {
        sprites = newSprites;
        PlayAnimation();
    }
}

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FillScreenSprite : MonoBehaviour
{
    void Start()
    {
        var cam = Camera.main;
        if (cam == null || !cam.orthographic)
        {
            Debug.LogWarning("FillScreenSprite requires an orthographic Camera tagged MainCamera.");
            return;
        }

        // 1. Compute world-space viewport dimensions
        float worldHeight = cam.orthographicSize * 2f;
        float worldWidth = worldHeight * cam.aspect;

        var sr = GetComponent<SpriteRenderer>();
        Vector2 sz = sr.sprite.bounds.size;
        float spriteW = sz.x;
        float spriteH = sz.y;

        // 3. Calculate uniform scale factor
        float scale = Mathf.Max(worldWidth / spriteW,
                                   worldHeight / spriteH);

        transform.localScale = new Vector3(scale, scale, 1f);
    }
}

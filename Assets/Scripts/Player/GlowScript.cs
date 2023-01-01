using UnityEngine;

public class GlowScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float glowIntensity = 1f;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        spriteRenderer.material.SetFloat("glow_intensity", glowIntensity);
    }
}

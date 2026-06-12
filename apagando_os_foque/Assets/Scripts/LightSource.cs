using UnityEngine;

public class LightSource : MonoBehaviour
{
    [Header("Configuração")]
    public float radius = 3f;
    public bool isBreakable = true;

    private CircleCollider2D lightCollider;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        lightCollider = GetComponent<CircleCollider2D>();
        lightCollider.radius = radius;
        lightCollider.isTrigger = true;

        // Tag da zona iluminada (crie a tag "IlluminatedZone" no Unity)
        gameObject.tag = "IlluminatedZone";
    }

    public void BreakLight()
    {
        if (!isBreakable) return;

        lightCollider.enabled = false; // desativa a zona de dano
        // Futuramente: tocar som, spawnar partícula
        gameObject.SetActive(false);
    }
}
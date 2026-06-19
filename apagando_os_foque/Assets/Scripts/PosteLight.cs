using UnityEngine;

public class PosteLight : MonoBehaviour
{
    [Header("Configuração")]
    public float radius = 6f; // raio maior que o lampião

    private CircleCollider2D lightCollider;

    void Awake()
    {
        lightCollider = GetComponent<CircleCollider2D>();
        if (lightCollider == null)
            lightCollider = gameObject.AddComponent<CircleCollider2D>();

        lightCollider.radius = radius;
        lightCollider.isTrigger = true;
        gameObject.tag = "IlluminatedZone";
        gameObject.layer = LayerMask.NameToLayer("LuzLayer");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        foreach (var enemy in FindObjectsOfType<EnemyAI>())
            enemy.SetPlayerIlluminated(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        foreach (var enemy in FindObjectsOfType<EnemyAI>())
            enemy.SetPlayerIlluminated(false);
    }
}
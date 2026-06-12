using UnityEngine;

public class LightSource : MonoBehaviour
{
    [Header("Configuração")]
    public float radius = 3f;
    public bool isBreakable = true;

    private CircleCollider2D lightCollider;

    void Awake()
    {
        lightCollider = GetComponent<CircleCollider2D>();
        lightCollider.radius = radius;
        lightCollider.isTrigger = true;
        gameObject.tag = "IlluminatedZone";
    }

    // Player entra na zona de luz
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Avisa todos os inimigos na cena
        foreach (var enemy in FindObjectsOfType<EnemyAI>())
            enemy.SetPlayerIlluminated(true);
    }

    // Player sai da zona de luz
    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (var enemy in FindObjectsOfType<EnemyAI>())
            enemy.SetPlayerIlluminated(false);
    }

    public void BreakLight()
    {
        if (!isBreakable) return;

        // Avisa inimigos que a luz sumiu
        foreach (var enemy in FindObjectsOfType<EnemyAI>())
            enemy.SetPlayerIlluminated(false);

        lightCollider.enabled = false;
        gameObject.SetActive(false);
    }
}
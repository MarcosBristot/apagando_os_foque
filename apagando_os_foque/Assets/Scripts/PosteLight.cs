using UnityEngine;

public class PosteLight : MonoBehaviour
{
    [Header("Configuração")]
    public float radius = 6f;
    public int vidaMaxima = 1; // Reduzido para 1 para sincronizar perfeitamente com a troca de sprite
    public int vidaAtual;

    private CircleCollider2D lightCollider;
    private LightSource lightSource;

    void Awake()
    {
        vidaAtual = vidaMaxima;

        lightCollider = GetComponent<CircleCollider2D>();
        if (lightCollider == null)
            lightCollider = gameObject.AddComponent<CircleCollider2D>();

        lightCollider.radius = radius;
        lightCollider.isTrigger = true;
        gameObject.tag = "IlluminatedZone";
        gameObject.layer = LayerMask.NameToLayer("LuzLayer");

        lightSource = GetComponent<LightSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
            enemy.SetPlayerIlluminated(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
            enemy.SetPlayerIlluminated(false);
    }

    public void BreakLight()
    {
        vidaAtual = 0; // Força a zerar a vida imediatamente

        foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
            enemy.SetPlayerIlluminated(false);

        LightManager.Instance?.RegistrarLuzApagada();

        if (lightSource != null)
            lightSource.BreakLight();

        lightCollider.enabled = false;
        
        // A linha gameObject.SetActive(false) foi removida para o sprite quebrado continuar visível
    }
}
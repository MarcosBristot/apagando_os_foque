using UnityEngine;

public class LightSource : MonoBehaviour
{
    [Header("Configuração")]
    public float radius = 4f;
    public bool isBreakable = true;
    public float intensidadeMaxima = 1f;

    private CircleCollider2D lightCollider;

    void Awake()
    {
        lightCollider = GetComponent<CircleCollider2D>();
        if (lightCollider == null)
            lightCollider = gameObject.AddComponent<CircleCollider2D>();

        lightCollider.radius = radius;
        lightCollider.isTrigger = true;
        gameObject.tag = "IlluminatedZone";
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

    public float GetIntensidadeNaPosicao(Vector2 posicao)
    {
        // Trava de segurança: se a colisão da luz foi desativada (luz quebrada), retorna 0 de dano
        if (lightCollider != null && !lightCollider.enabled) return 0f;

        float distancia = Vector2.Distance(transform.position, posicao);
        float normalizado = Mathf.Clamp01(1f - (distancia / radius));
        return normalizado * intensidadeMaxima;
    }

    public void BreakLight()
    {
        if (!isBreakable) return;

        foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
            enemy.SetPlayerIlluminated(false);

        // Notifica o gerenciador de luzes
        LightManager.Instance?.RegistrarLuzApagada();

        // Apenas desativa a colisão para parar de dar dano, MAS NÃO desativa o gameObject
        if (lightCollider != null)
        {
            lightCollider.enabled = false;
        }
    }
}
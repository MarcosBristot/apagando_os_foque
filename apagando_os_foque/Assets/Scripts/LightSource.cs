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
        foreach (var enemy in FindObjectsOfType<EnemyAI>())
            enemy.SetPlayerIlluminated(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        foreach (var enemy in FindObjectsOfType<EnemyAI>())
            enemy.SetPlayerIlluminated(false);
    }

    // retorna 0 a 1 — mais perto do centro = mais próximo de 1
    public float GetIntensidadeNaPosicao(Vector2 posicao)
    {
        float distancia = Vector2.Distance(transform.position, posicao);
        float normalizado = Mathf.Clamp01(1f - (distancia / radius));
        return normalizado * intensidadeMaxima;
    }

    public void BreakLight()
    {
        if (!isBreakable) return;

        foreach (var enemy in FindObjectsOfType<EnemyAI>())
            enemy.SetPlayerIlluminated(false);

        // notifica o gerenciador de luzes
        LightManager.Instance?.RegistrarLuzApagada();

        lightCollider.enabled = false;
        gameObject.SetActive(false);
    }
}

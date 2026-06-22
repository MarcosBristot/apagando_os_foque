using UnityEngine;

public class PosteLight : MonoBehaviour
{
    [Header("Configuração")]
    public float radius = 6f;
    public int vidaMaxima = 2;
    private int vidaAtual;

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
        foreach (var enemy in FindObjectsOfType<EnemyAI>())
            enemy.SetPlayerIlluminated(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        foreach (var enemy in FindObjectsOfType<EnemyAI>())
            enemy.SetPlayerIlluminated(false);
    }

    public void BreakLight()
    {
        vidaAtual--;

        if (vidaAtual > 0) return; // ainda tem vida, não apaga

        foreach (var enemy in FindObjectsOfType<EnemyAI>())
            enemy.SetPlayerIlluminated(false);

        LightManager.Instance?.RegistrarLuzApagada();

        if (lightSource != null)
            lightSource.BreakLight();

        lightCollider.enabled = false;
        gameObject.SetActive(false);
    }
}
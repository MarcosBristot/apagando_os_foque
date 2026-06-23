using UnityEngine;

public class LampiaoCaipira : MonoBehaviour
{
    [Header("Configuração")]
    public float radius = 2.5f;
    public bool isBreakable = true;

    private CircleCollider2D lightCollider;
    private LightSource lightSource;

    void Awake()
    {
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
        foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None ))            enemy.SetPlayerIlluminated(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None ))            enemy.SetPlayerIlluminated(false);
    }

    public void BreakLight()
    {
        if (!isBreakable) return;

        foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None   ))
            enemy.SetPlayerIlluminated(false);

        LightManager.Instance?.RegistrarLuzApagada();

        // apaga o LightSource também para parar o dano de stamina
        if (lightSource != null)
            lightSource.BreakLight();

        lightCollider.enabled = false;
    }
}
using UnityEngine;

public class LampiaoCaipira : MonoBehaviour
{
    [Header("Configuração")]
    public float radius = 2.5f; // raio menor que o poste
    public bool isBreakable = true;

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

    public void BreakLight()
    {
        if (!isBreakable) return;

        foreach (var enemy in FindObjectsOfType<EnemyAI>())
            enemy.SetPlayerIlluminated(false);

        lightCollider.enabled = false;
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D_Attack(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
            BreakLight();
    }
}
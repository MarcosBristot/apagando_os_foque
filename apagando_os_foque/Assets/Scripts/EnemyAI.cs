using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Patrol, Alert, Chase }
    public EnemyState currentState = EnemyState.Patrol;

    [Header("Patrulha")]
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float waypointTolerance = 0.2f;

    [Header("Detecção")]
    public float detectionRadius = 4f;

    private int currentWaypointIndex = 0;
    private Transform player;
    private bool playerIsIlluminated = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol: HandlePatrol(); break;
            case EnemyState.Alert:  HandleAlert();  break;
            case EnemyState.Chase:  HandleChase();  break;
        }
    }

    void HandlePatrol()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(
            transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < waypointTolerance)
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

        DetectPlayer();
    }

    void HandleAlert()
    {
        // Fica em alerta por 2 segundos, depois volta a patrulhar
        Invoke(nameof(ReturnToPatrol), 2f);
    }

    void HandleChase()
    {
        if (player == null) return;

        // Para de perseguir se o player sair da luz
        if (!playerIsIlluminated)
        {
            currentState = EnemyState.Alert;
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position, player.position, moveSpeed * 1.5f * Time.deltaTime);
    }

    void DetectPlayer()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Só persegue se o player estiver iluminado E dentro do raio
        if (dist <= detectionRadius && playerIsIlluminated)
            currentState = EnemyState.Chase;
    }

    void ReturnToPatrol()
    {
        if (currentState == EnemyState.Alert)
            currentState = EnemyState.Patrol;
    }

    // Chamado pelo LightSource quando o player entra/sai da zona iluminada
    public void SetPlayerIlluminated(bool illuminated)
    {
        playerIsIlluminated = illuminated;

        if (illuminated && currentState == EnemyState.Patrol)
            currentState = EnemyState.Alert;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
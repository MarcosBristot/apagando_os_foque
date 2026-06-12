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

    void Start()
    {
        // Encontra o player pelo nome ou tag
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
        // Fica parado por um tempo, olha ao redor — stub por enquanto
        // No Sprint 2 vira Chase se o player entrar na zona iluminada
        currentState = EnemyState.Patrol;
    }

    void HandleChase()
    {
        if (player == null) return;
        transform.position = Vector2.MoveTowards(
            transform.position, player.position, moveSpeed * 1.5f * Time.deltaTime);
    }

    void DetectPlayer()
    {
        if (player == null) return;

        Collider2D hit = Physics2D.OverlapCircle(
            transform.position, detectionRadius, LayerMask.GetMask("Player"));

        if (hit != null)
            currentState = EnemyState.Alert;
    }

    // Visualiza o raio de detecção no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
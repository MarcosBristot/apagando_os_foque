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

    [Header("Alerta")]
    public float alertDuration = 2f;

    private int currentWaypointIndex = 0;
    private Transform player;
    private bool playerIsIlluminated = false;
    private float alertTimer = 0f;

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
        // conta o tempo em alerta usando deltaTime, sem Invoke
        alertTimer += Time.deltaTime;

        if (alertTimer >= alertDuration)
        {
            alertTimer = 0f;
            currentState = EnemyState.Patrol;
        }
    }

    void HandleChase()
    {
        if (player == null) return;

        if (!playerIsIlluminated)
        {
            alertTimer = 0f;
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

        if (dist <= detectionRadius && playerIsIlluminated)
        {
            alertTimer = 0f;
            currentState = EnemyState.Chase;
        }
    }

    public void SetPlayerIlluminated(bool illuminated)
    {
        playerIsIlluminated = illuminated;

        if (illuminated && currentState == EnemyState.Patrol)
        {
            alertTimer = 0f;
            currentState = EnemyState.Alert;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
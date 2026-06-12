using UnityEngine;

// Stub do GuardController — será expandido no Sprint 2
// Por enquanto herda o comportamento base de EnemyAI
[RequireComponent(typeof(EnemyAI))]
public class GuardController : MonoBehaviour
{
    private EnemyAI enemyAI;

    void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
    }

    // Exposição dos estados para outros sistemas (ex: StaminaSystem do Dev 3)
    public EnemyAI.EnemyState CurrentState => enemyAI.currentState;

    public void ForceAlert() => enemyAI.currentState = EnemyAI.EnemyState.Alert;
    public void ForceChase() => enemyAI.currentState = EnemyAI.EnemyState.Chase;
    public void ForcePatrol() => enemyAI.currentState = EnemyAI.EnemyState.Patrol;
}
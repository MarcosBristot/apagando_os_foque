using UnityEngine;
using UnityEngine.UI;

public class AlertIndicator : MonoBehaviour
{
    [Header("Ícone de alerta")]
    public GameObject alertIcon; // arraste um sprite de "!" aqui no Inspector

    private EnemyAI enemyAI;

    void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();

        if (alertIcon != null)
            alertIcon.SetActive(false);
    }

    void Update()
    {
        if (alertIcon == null) return;

        // Mostra "!" quando em Alert ou Chase
        bool showAlert = enemyAI.currentState == EnemyAI.EnemyState.Alert
                      || enemyAI.currentState == EnemyAI.EnemyState.Chase;

        alertIcon.SetActive(showAlert);
    }
}
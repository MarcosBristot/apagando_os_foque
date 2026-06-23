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
    public float alertDuration = 2f;

    [Header("Vida")]
    public int vidaMaxima = 3;
    private int vidaAtual;
    private bool morto = false;

    [Header("Ataque")]
    public float danoAoPlayer = 10f;
    public float cooldownAtaque = 1.5f;
    private float timerAtaque = 0f;

    private int currentWaypointIndex = 0;
    private Transform player;
    private AdelarTopDownController playerController;
    private float alertTimer = 0f;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector3 escalaOriginal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        vidaAtual = vidaMaxima;

        anim = GetComponent<Animator>();
        escalaOriginal = transform.localScale;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerController = playerObj.GetComponent<AdelarTopDownController>();
        }
    }

    void Update()
    {
        if (morto) return;
        CalcularAnimacao();
    }

    void FixedUpdate()
    {
        if (morto)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        switch (currentState)
        {
            case EnemyState.Patrol: HandlePatrol(); break;
            case EnemyState.Alert:  HandleAlert();  break;
            case EnemyState.Chase:  HandleChase();  break;
        }
    }

    void CalcularAnimacao()
    {
        if (anim == null) return;

        Vector2 velocidadeAtual = rb.linearVelocity;
        Vector2 direcaoNormalizada = velocidadeAtual.normalized;

        anim.SetFloat("MoveX", direcaoNormalizada.x);
        anim.SetFloat("MoveY", direcaoNormalizada.y);
        anim.SetFloat("Speed", velocidadeAtual.sqrMagnitude);

        if (velocidadeAtual.sqrMagnitude > 0.01f)
        {
            anim.SetFloat("LastMoveX", direcaoNormalizada.x);
            anim.SetFloat("LastMoveY", direcaoNormalizada.y);

            if (direcaoNormalizada.x != 0)
            {
                float direcaoX = Mathf.Sign(direcaoNormalizada.x);
                transform.localScale = new Vector3(
                    direcaoX * Mathf.Abs(escalaOriginal.x),
                    escalaOriginal.y,
                    escalaOriginal.z
                );
            }
        }
    }

    void HandlePatrol()
    {
        if (waypoints.Length == 0) { rb.linearVelocity = Vector2.zero; return; }

        Transform target = waypoints[currentWaypointIndex];
        Vector2 direcao = ((Vector2)target.position - rb.position).normalized;
        float dist = Vector2.Distance(rb.position, target.position);

        if (dist < waypointTolerance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = direcao * moveSpeed;

        if (PlayerEstaIluminadoPerto())
        {
            rb.linearVelocity = Vector2.zero;
            alertTimer = alertDuration;
            currentState = EnemyState.Alert;
        }
    }

    void HandleAlert()
    {
        rb.linearVelocity = Vector2.zero;
        alertTimer -= Time.fixedDeltaTime;

        if (alertTimer <= 0f)
        {
            currentWaypointIndex = EncontrarWaypointMaisProximo();
            currentState = EnemyState.Patrol;
            return;
        }

        if (PlayerEstaIluminadoPerto())
            currentState = EnemyState.Chase;
    }

    void HandleChase()
    {
        if (player == null) { rb.linearVelocity = Vector2.zero; return; }

        if (!PlayerEstaIluminado())
        {
            rb.linearVelocity = Vector2.zero;
            alertTimer = alertDuration;
            currentState = EnemyState.Alert;
            return;
        }

        float dist = Vector2.Distance(rb.position, player.position);

        if (dist > detectionRadius * 2.5f)
        {
            rb.linearVelocity = Vector2.zero;
            alertTimer = alertDuration;
            currentState = EnemyState.Alert;
            return;
        }

        if (dist < 0.8f)
        {
            rb.linearVelocity = Vector2.zero;
            timerAtaque -= Time.fixedDeltaTime;
            if (timerAtaque <= 0f)
            {
                timerAtaque = cooldownAtaque;
                StaminaSystem stamina = player.GetComponent<StaminaSystem>();
                if (stamina != null)
                    stamina.ReceberDano(danoAoPlayer);
            }
            return;
        }

        Vector2 direcao = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = direcao * moveSpeed * 1.5f;
    }

    public void ReceberDano(int dano = 1)
    {
        if (morto) return;
        vidaAtual -= dano;
        if (vidaAtual <= 0) Morrer();
    }

    void Morrer()
    {
        morto = true;
        rb.linearVelocity = Vector2.zero;
        foreach (var col in GetComponents<Collider2D>())
            col.enabled = false;
        Destroy(gameObject, 0.1f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
            ReceberDano(1);
    }

    bool PlayerEstaIluminadoPerto()
    {
        if (player == null || playerController == null) return false;
        return playerController.naLuz &&
               Vector2.Distance(rb.position, player.position) <= detectionRadius;
    }

    bool PlayerEstaIluminado()
    {
        if (playerController == null) return false;
        return playerController.naLuz;
    }

    int EncontrarWaypointMaisProximo()
    {
        if (waypoints.Length == 0) return 0;
        int indice = 0;
        float menorDist = float.MaxValue;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            float dist = Vector2.Distance(rb.position, waypoints[i].position);
            if (dist < menorDist) { menorDist = dist; indice = i; }
        }
        return indice;
    }

    public void SetPlayerIlluminated(bool illuminated) { }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
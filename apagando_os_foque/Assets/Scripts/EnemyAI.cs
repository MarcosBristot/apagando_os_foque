using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Patrol, Alert, Chase, FixLight }
    public EnemyState currentState = EnemyState.Patrol;

    [Header("Patrulha")]
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float waypointTolerance = 0.2f;
    public float raioBuscaLuz = 5f; 
    public float tempoParaConsertar = 2f;
    [Tooltip("Distância que ele precisa chegar da luz para consertar")]
    public float distanciaParaConserto = 1.5f;

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

    [Tooltip("Tempo em segundos que ele ignora lâmpadas após consertar uma")]
    public float cooldownEntreConsertos = 4f; 
    private float timerCooldownAtual = 0f;
    // Variáveis de Conserto
    private Lantern lampadaAlvo;
    private float timerConsertoAtual = 0f;

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

        // REDUZ O COOLDOWN: Isso garante que ele volte a procurar luzes depois de um tempo
        if (timerCooldownAtual > 0f)
        {
            timerCooldownAtual -= Time.fixedDeltaTime;
        }

        switch (currentState)
        {
            case EnemyState.Patrol: HandlePatrol(); break;
            case EnemyState.Alert:  HandleAlert();  break;
            case EnemyState.Chase:  HandleChase();  break;
            case EnemyState.FixLight: HandleFixLight(); break; // <-- A LINHA QUE FALTAVA!
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
        // NOVA LÓGICA: Procura todas as Lanterns na cena, ignorando colisões físicas
        if (timerCooldownAtual <= 0f)
        {
            Lantern[] todasAsLuzes = FindObjectsByType<Lantern>(FindObjectsSortMode.None);
            foreach (Lantern luz in todasAsLuzes)
            {
                // Checa se está quebrada E se está dentro do raio de visão do inimigo
                if (luz.estaQuebrada && Vector2.Distance(rb.position, luz.transform.position) <= raioBuscaLuz)
                {
                    lampadaAlvo = luz;
                    currentState = EnemyState.FixLight;
                    return;
                }
            }
        }

        // Continua a patrulha normal se não achar luzes[cite: 15]
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

void HandleFixLight()
    {
        // Se a lâmpada sumir ou for consertada por outro
        if (lampadaAlvo == null || !lampadaAlvo.estaQuebrada)
        {
            currentState = EnemyState.Patrol;
            return;
        }

        float dist = Vector2.Distance(rb.position, lampadaAlvo.transform.position);

        // Se ainda está mais longe que a distância de conserto, caminha até lá
        if (dist > distanciaParaConserto) 
        {
            Vector2 direcao = ((Vector2)lampadaAlvo.transform.position - rb.position).normalized;
            rb.linearVelocity = direcao * moveSpeed;
        }
        else // Chegou na lâmpada
        {
            rb.linearVelocity = Vector2.zero; // Para o personagem
            
            if (timerConsertoAtual == 0f && anim != null)
            {
                anim.SetTrigger("Consertar"); // Chama a animação
            }

            timerConsertoAtual += Time.fixedDeltaTime;

            if (timerConsertoAtual >= tempoParaConsertar)
            {
                lampadaAlvo.Consertar(); // Conserta fisicamente a luz
                timerConsertoAtual = 0f;
                lampadaAlvo = null;
                
                // Aplica o cooldown para não consertar outra luz imediatamente
                timerCooldownAtual = cooldownEntreConsertos; 
                
                currentState = EnemyState.Patrol; // Volta a patrulhar
            }
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
using UnityEngine;

public class AdelarTopDownController : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidadeNormal = 4f;
    public float velocidadeSombra = 7f;

    [Header("Detecção de Luz")]
    public LayerMask camadaDeLuz;
    public float raioDeteccao = 0.5f;

    [Header("Ataques")]
    public float raioAtaqueLuz = 1.5f;    // raio para atacar luzes — tecla E
    public float raioAtaqueInimigo = 1.5f;  // raio para atacar inimigos — tecla F
    public KeyCode teclaAtaqueLuz = KeyCode.E;
    public KeyCode teclaAtaqueInimigo = KeyCode.F;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movimento;
    private StaminaSystem stamina;

    public bool naLuz = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        stamina = GetComponent<StaminaSystem>();
    }

    void Update()
    {
        movimento.x = Input.GetAxisRaw("Horizontal");
        movimento.y = Input.GetAxisRaw("Vertical");

        anim.SetFloat("MoveX", movimento.x);
        anim.SetFloat("MoveY", movimento.y);
        anim.SetFloat("Speed", movimento.sqrMagnitude);

        if (movimento.x != 0 || movimento.y != 0)
        {
            anim.SetFloat("LastMoveX", movimento.x);
            anim.SetFloat("LastMoveY", movimento.y);

            if (movimento.x != 0)
                transform.localScale = new Vector3(Mathf.Sign(movimento.x), 1, 1);
        }

        // E — ataca luzes
        if (Input.GetKeyDown(teclaAtaqueLuz))
            AtacarLuz();

        // F — ataca inimigos
        if (Input.GetKeyDown(teclaAtaqueInimigo))
            AtacarInimigo();

        VerificarLuz();
    }

    void FixedUpdate()
    {
        float velocidade = naLuz ? velocidadeNormal : velocidadeSombra;
        rb.linearVelocity = movimento.normalized * velocidade;
    }

    // tecla E — só acerta luzes
    void AtacarLuz()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, raioAtaqueLuz);
        foreach (var hit in hits)
        {
            Lantern lanterna = hit.GetComponent<Lantern>();
            if (lanterna != null)
                lanterna.TakeHit();
        }
    }

    // tecla F — só acerta inimigos
    void AtacarInimigo()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, raioAtaqueInimigo);
        foreach (var hit in hits)
        {
            EnemyAI inimigo = hit.GetComponent<EnemyAI>();
            if (inimigo != null)
                inimigo.ReceberDano(1);
        }
    }

    void VerificarLuz()
    {
        Collider2D[] colisoes = Physics2D.OverlapCircleAll(
            transform.position, raioDeteccao, camadaDeLuz);

        bool estaVendoLuz = colisoes.Length > 0;

        if (estaVendoLuz && !naLuz) EntrarNaLuz();
        if (!estaVendoLuz && naLuz) SairDaLuz();

        if (estaVendoLuz)
        {
            float maiorIntensidade = 1f;
            foreach (Collider2D col in colisoes)
            {
                LightSource fonte = col.GetComponent<LightSource>();
                if (fonte != null)
                {
                    float intensidade = fonte.GetIntensidadeNaPosicao(transform.position);
                    if (intensidade > maiorIntensidade)
                        maiorIntensidade = intensidade;
                }
            }
            stamina?.SetIntensidadeLuz(maiorIntensidade);
        }
    }

    public void EntrarNaLuz()
    {
        naLuz = true;
        stamina?.EntrarNaLuz();
    }

    public void SairDaLuz()
    {
        naLuz = false;
        stamina?.SairDaLuz();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioDeteccao);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioAtaqueInimigo);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, raioAtaqueLuz);
    }
}
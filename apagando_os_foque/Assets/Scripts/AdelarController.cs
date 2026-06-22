using UnityEngine;

public class AdelarTopDownController : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidadeNormal = 4f;
    public float velocidadeSombra = 7f;

    [Header("Detecção de Luz")]
    public LayerMask camadaDeLuz;
    public float raioDeteccao = 0.5f;

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
        // input de movimento
        movimento.x = Input.GetAxisRaw("Horizontal");
        movimento.y = Input.GetAxisRaw("Vertical");

        // animações
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

        // ataque
        if (Input.GetKeyDown(KeyCode.E))
            Atacar();

        VerificarLuz();
    }

    void FixedUpdate()
    {
        float velocidade = naLuz ? velocidadeNormal : velocidadeSombra;
        rb.linearVelocity = movimento.normalized * velocidade;
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
            float maiorIntensidade = 0f;
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

    public void Atacar()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var hit in hits)
        {
            Lantern lanterna = hit.GetComponent<Lantern>();
            if (lanterna != null)
                lanterna.TakeHit();

            EnemyAI inimigo = hit.GetComponent<EnemyAI>();
            if (inimigo != null)
                inimigo.ReceberDano(1);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioDeteccao);
    }
}
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidadeNormal = 4f;
    public float velocidadeSombra = 7f;

    [Header("Detecção de Luz")]
    public LayerMask camadaDeLuz;
    public float raioDeteccao = 0.5f;

    private Rigidbody2D rb;
    private StaminaSystem stamina;
    public bool naLuz = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        stamina = GetComponent<StaminaSystem>();
    }

    void Update()
    {
        Mover();
        VerificarLuz();
    }

    void Mover()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 direcao = new Vector2(x, y).normalized;
        float velocidade = naLuz ? velocidadeNormal : velocidadeSombra;

        rb.linearVelocity = direcao * velocidade;
    }

    void VerificarLuz()
    {
        Collider2D[] colisoes = Physics2D.OverlapCircleAll(
            transform.position,
            raioDeteccao,
            camadaDeLuz
        );

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

    // chamado por ataque (Sprint 2)
   public void Atacar()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var hit in hits)
        {
            // tenta acertar lampião
            Lantern lanterna = hit.GetComponent<Lantern>();
            if (lanterna != null)
                lanterna.TakeHit();

            // tenta acertar inimigo
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

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidadeNormal = 4f;
    public float velocidadeSombra = 7f;

    [Header("Stamina")]
    public float staminaMax = 100f;
    public float staminaAtual;
    public float drenagemStamina = 15f;  // por segundo na luz
    public float regeneracaoStamina = 10f; // por segundo na sombra

    [Header("Vida")]
    public float vidaMax = 100f;
    public float vidaAtual;
    public float danoPorLuz = 10f; // por segundo, só quando stamina = 0

    private Rigidbody2D rb;
    private bool naLuz = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        staminaAtual = staminaMax;
        vidaAtual = vidaMax;
    }

    void Update()
    {
        Mover();
        VerificarLuz();
        AtualizarLuz();
    }
    
    void Mover()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 direcao = new Vector2(x, y).normalized;
        float velocidade = naLuz ? velocidadeNormal : velocidadeSombra;

        rb.linearVelocity = direcao * velocidade;
    }

    void AtualizarLuz()
    {
        if (naLuz)
        {
            // drena stamina primeiro
            if (staminaAtual > 0)
            {
                staminaAtual -= drenagemStamina * Time.deltaTime;
                staminaAtual = Mathf.Max(staminaAtual, 0);
            }
            else
            {
                // stamina zerada: começa a perder vida
                vidaAtual -= danoPorLuz * Time.deltaTime;
                vidaAtual = Mathf.Max(vidaAtual, 0);

                if (vidaAtual <= 0)
                    Morrer();
            }
        }
        else
        {
            // na sombra: regenera stamina
            staminaAtual += regeneracaoStamina * Time.deltaTime;
            staminaAtual = Mathf.Min(staminaAtual, staminaMax);
        }
    }

    // Chamado pelo sistema de luz quando o Adelar entra/sai da luz
    public void EntrarNaLuz() => naLuz = true;
    public void SairDaLuz()   => naLuz = false;

    void Morrer()
    {
        Debug.Log("Adelar morreu!");
        // aqui vamos chamar o GameManager futuramente
    }

    [Header("Detecção de Luz")]
    public LayerMask camadaDeLuz;
    public float raioDeteccao = 0.3f;

    void VerificarLuz()
    {
        // pega todos os colliders de luz ao redor do Adelar
        Collider2D[] colisoes = Physics2D.OverlapCircleAll(
            transform.position,
            raioDeteccao,
            camadaDeLuz
        );

        naLuz = colisoes.Length > 0;
    }
}
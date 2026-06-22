using UnityEngine;

public class AdelarTopDownController : MonoBehaviour
{
    public float velocidade = 5f;
    private Animator anim;
    private Vector2 movimento;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Captura os inputs de todas as direções (WASD ou Setas)
        movimento.x = Input.GetAxisRaw("Horizontal");
        movimento.y = Input.GetAxisRaw("Vertical");

        // 2. Envia os valores atuais para o Animator
        anim.SetFloat("MoveX", movimento.x);
        anim.SetFloat("MoveY", movimento.y);
        anim.SetFloat("Speed", movimento.sqrMagnitude);

        // 3. Salva a última direção olhada para o Idle funcionar corretamente
        if (movimento.x != 0 || movimento.y != 0)
        {
            anim.SetFloat("LastMoveX", movimento.x);
            anim.SetFloat("LastMoveY", movimento.y);

            // Espelha o sprite caso você use a mesma animação para Direita e Esquerda
            if (movimento.x != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(movimento.x), 1, 1);
            }
        }
    }

    void FixedUpdate()
    {
        // Movimenta o personagem fisicamente pela tela
        transform.Translate(movimento.normalized * velocidade * Time.fixedDeltaTime);
    }
}
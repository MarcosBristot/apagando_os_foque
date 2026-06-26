using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 ultimaDirecao = Vector2.down;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 velocidade = rb.linearVelocity;
        bool movendo = velocidade.magnitude > 0.1f;

        if (movendo)
        {
            // descobre direção predominante
            if (Mathf.Abs(velocidade.x) > Mathf.Abs(velocidade.y))
                ultimaDirecao = velocidade.x > 0 ? Vector2.right : Vector2.left;
            else
                ultimaDirecao = velocidade.y > 0 ? Vector2.up : Vector2.down;
        }

        // monta o nome da animação: "run_down", "idle_right", etc
        string direcao = GetDirecaoString(ultimaDirecao);
        string estado = movendo ? "run" : "idle";
        animator.Play($"{estado}_{direcao}");
    }

    string GetDirecaoString(Vector2 dir)
    {
        if (dir == Vector2.up)    return "up";
        if (dir == Vector2.down)  return "down";
        if (dir == Vector2.left)  return "left";
        if (dir == Vector2.right) return "right";
        return "down";
    }
}
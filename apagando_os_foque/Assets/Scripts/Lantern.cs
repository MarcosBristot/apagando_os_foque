using UnityEngine;

public class Lantern : MonoBehaviour
{
    private LightSource lightSource;

    void Awake()
    {
        lightSource = GetComponent<LightSource>();
    }

    // Chamado pelo ataque do Adelar (o Líder vai conectar isso ao PlayerController)
    public void TakeHit()
    {
        if (lightSource != null)
            lightSource.BreakLight();
    }

    // Detecta colisão com o ataque do player
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
            TakeHit();
    }
}
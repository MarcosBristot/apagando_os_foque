using UnityEngine;

public class Lantern : MonoBehaviour
{
    [Header("Visuais da Lâmpada")]
    public SpriteRenderer spriteRenderer; 
    public Sprite spriteAcesa;
    public Sprite spriteApagada;

    private LightSource lightSource;
    private LampiaoCaipira lampiaoCaipira;
    private PosteLight posteLight;
    
    public bool estaQuebrada = false;

    void Awake()
    {
        lightSource = GetComponent<LightSource>();
        lampiaoCaipira = GetComponent<LampiaoCaipira>();
        posteLight = GetComponent<PosteLight>();

        // Tenta achar o SpriteRenderer automaticamente caso você esqueça de arrastar no Inspector
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Garante que o jogo comece com o sprite aceso correto
        if (spriteRenderer != null && spriteAcesa != null && !estaQuebrada)
        {
            spriteRenderer.sprite = spriteAcesa;
        }
    }

    public void TakeHit()
    {
        if (estaQuebrada) return; 

        estaQuebrada = true;

        // TROCA O SPRITE PARA APAGADO
        if (spriteRenderer != null && spriteApagada != null)
        {
            spriteRenderer.sprite = spriteApagada;
        }

        if (lightSource != null && lampiaoCaipira == null && posteLight == null)
            lightSource.BreakLight();

        if (lampiaoCaipira != null)
            lampiaoCaipira.BreakLight();

        if (posteLight != null)
            posteLight.BreakLight();
    }

    public void Consertar()
    {
        if (!estaQuebrada) return;

        estaQuebrada = false;
        LightManager.Instance?.RegistrarLuzConsertada();

        // TROCA O SPRITE DE VOLTA PARA ACESO
        if (spriteRenderer != null && spriteAcesa != null)
        {
            spriteRenderer.sprite = spriteAcesa;
        }

        // Reativa a colisão e a vida
        if (posteLight != null)
        {
            posteLight.vidaAtual = posteLight.vidaMaxima;
            posteLight.GetComponent<CircleCollider2D>().enabled = true;
        }
        else if (lampiaoCaipira != null)
        {
            lampiaoCaipira.GetComponent<CircleCollider2D>().enabled = true;
        }
        else if (lightSource != null)
        {
            lightSource.GetComponent<CircleCollider2D>().enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
            TakeHit();
    }
}
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

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && spriteAcesa != null && !estaQuebrada)
            spriteRenderer.sprite = spriteAcesa;
    }

    void AplicarSpriteApagado()
    {
        if (spriteRenderer != null && spriteApagada != null)
            spriteRenderer.sprite = spriteApagada;
    }

    public void TakeHit()
    {
        if (posteLight != null)
        {
            posteLight.BreakLight();
            if (posteLight.vidaAtual <= 0)
            {
                estaQuebrada = true;
                AplicarSpriteApagado();
            }
            return;
        }

        if (estaQuebrada) return;

        estaQuebrada = true;
        AplicarSpriteApagado();

        if (lightSource != null && lampiaoCaipira == null)
            lightSource.BreakLight();

        if (lampiaoCaipira != null)
            lampiaoCaipira.BreakLight();
    }

    public void Consertar()
    {
        if (!estaQuebrada) return;

        estaQuebrada = false;
        LightManager.Instance?.RegistrarLuzConsertada();

        if (spriteRenderer != null && spriteAcesa != null)
            spriteRenderer.sprite = spriteAcesa;

        if (posteLight != null)
        {
            posteLight.vidaAtual = posteLight.vidaMaxima;
            posteLight.GetComponent<CircleCollider2D>().enabled = true;
            SpriteRenderer sr = posteLight.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = true;
        }
        else if (lampiaoCaipira != null)
        {
            lampiaoCaipira.GetComponent<CircleCollider2D>().enabled = true;
            SpriteRenderer sr = lampiaoCaipira.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = true;
        }
        else if (lightSource != null)
        {
            lightSource.GetComponent<CircleCollider2D>().enabled = true;
            SpriteRenderer sr = lightSource.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
            TakeHit();
    }
}
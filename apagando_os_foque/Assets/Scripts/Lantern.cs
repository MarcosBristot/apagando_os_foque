using UnityEngine;
using UnityEngine.Rendering.Universal; // Importante para controlar a Luz 2D do URP

public class Lantern : MonoBehaviour
{
    [Header("Visuais da Lâmpada")]
    public SpriteRenderer spriteRenderer; 
    public Sprite spriteAcesa;
    public Sprite spriteApagada;
    
    [Header("Luz Visual 2D")]
    public Light2D luzVisual; // Arraste a Point Light 2D para cá no Inspector

    private LightSource lightSource;
    private LampiaoCaipira lampiaoCaipira;
    private PosteLight posteLight;
    
    public bool estaQuebrada = false;

    void Awake()
    {
        lightSource = GetComponent<LightSource>();
        lampiaoCaipira = GetComponent<LampiaoCaipira>();
        posteLight = GetComponent<PosteLight>();

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (luzVisual == null) luzVisual = GetComponentInChildren<Light2D>();

        if (spriteRenderer != null && spriteAcesa != null && !estaQuebrada)
            spriteRenderer.sprite = spriteAcesa;
    }

    public void TakeHit()
    {
        if (estaQuebrada) return; 

        estaQuebrada = true;

        if (spriteRenderer != null && spriteApagada != null)
            spriteRenderer.sprite = spriteApagada;

        // DESLIGA A LUZ VISUAL
        if (luzVisual != null) luzVisual.enabled = false;

        if (lightSource != null && lampiaoCaipira == null && posteLight == null)
            lightSource.BreakLight();

        if (lampiaoCaipira != null) lampiaoCaipira.BreakLight();
        if (posteLight != null) posteLight.BreakLight();
    }

    public void Consertar()
    {
        if (!estaQuebrada) return;

        estaQuebrada = false;
        LightManager.Instance?.RegistrarLuzConsertada();

        if (spriteRenderer != null && spriteAcesa != null)
            spriteRenderer.sprite = spriteAcesa;

        // LIGA A LUZ VISUAL DE VOLTA
        if (luzVisual != null) luzVisual.enabled = true;

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
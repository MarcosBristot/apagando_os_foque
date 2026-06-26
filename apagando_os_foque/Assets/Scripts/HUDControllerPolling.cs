using UnityEngine;

/// <summary>
/// Versão ALTERNATIVA do HUDController que funciona por polling (Update).
/// Use este se não puder modificar o StaminaSystem para passar float nos eventos.
///
/// CONFIGURAÇÃO NO INSPECTOR:
/// - bloodSpriteRenderer:   SpriteRenderer da barra de vida
/// - bateriaSpriteRenderer: SpriteRenderer da barra de stamina
/// - bloodSprites:    Sprites da barra de vida (índice 0 = cheia → último = vazia)
/// - bateriaSprites:  Sprites da bateria     (índice 0 = cheia → último = vazia)
/// - staminaSystem:   Referência ao StaminaSystem do Adelar
/// - intervaloUpdate: quão frequente atualiza (0.05 = 20x por segundo, padrão)
/// </summary>
public class HUDControllerPolling : MonoBehaviour
{
    [Header("Referências dos Sprite Renderers")]
    public SpriteRenderer bloodSpriteRenderer;
    public SpriteRenderer bateriaSpriteRenderer;

    [Header("Sprites das Barras")]
    [Tooltip("13 frames: índice 0 = vida cheia, índice 12 = morto")]
    public Sprite[] bloodSprites;

    [Tooltip("12 frames: índice 0 = bateria cheia, índice 11 = descarregada")]
    public Sprite[] bateriaSprites;

    [Header("Sistema de Status")]
    public StaminaSystem staminaSystem;

    [Header("Performance")]
    [Range(0.016f, 0.5f)]
    [Tooltip("Intervalo de atualização em segundos. 0.05 = 20fps para a HUD")]
    public float intervaloUpdate = 0.05f;

    // Controle interno
    private float timerUpdate = 0f;
    private int indiceVidaAtual = -1;
    private int indiceStaminaAtual = -1;

    void Start()
    {
        if (staminaSystem == null)
        {
            staminaSystem = FindObjectOfType<StaminaSystem>();

            if (staminaSystem == null)
            {
                Debug.LogError("[HUDControllerPolling] StaminaSystem não encontrado!");
                enabled = false;
                return;
            }
        }

        // Atualiza imediatamente no início
        AtualizarHUD();
    }

    void Update()
    {
        timerUpdate += Time.deltaTime;

        if (timerUpdate >= intervaloUpdate)
        {
            timerUpdate = 0f;
            AtualizarHUD();
        }
    }

    private void AtualizarHUD()
    {
        // ── Barra de Vida ────────────────────────────────────────────────
        if (bloodSprites != null && bloodSprites.Length > 0 && bloodSpriteRenderer != null)
        {
            float pctVida = staminaSystem.vidaMax > 0
                ? staminaSystem.vidaAtual / staminaSystem.vidaMax
                : 0f;

            int novoIndiceVida = CalcularIndice(pctVida, bloodSprites.Length);

            if (novoIndiceVida != indiceVidaAtual)
            {
                indiceVidaAtual = novoIndiceVida;
                bloodSpriteRenderer.sprite = bloodSprites[indiceVidaAtual];
            }
        }

        // ── Barra de Stamina ─────────────────────────────────────────────
        if (bateriaSprites != null && bateriaSprites.Length > 0 && bateriaSpriteRenderer != null)
        {
            float pctStamina = staminaSystem.staminaMax > 0
                ? staminaSystem.staminaAtual / staminaSystem.staminaMax
                : 0f;

            int novoIndiceStamina = CalcularIndice(pctStamina, bateriaSprites.Length);

            if (novoIndiceStamina != indiceStaminaAtual)
            {
                indiceStaminaAtual = novoIndiceStamina;
                bateriaSpriteRenderer.sprite = bateriaSprites[indiceStaminaAtual];
            }
        }
    }

    /// <summary>
    /// 100% → frame 0 (cheio), 0% → último frame (vazio)
    /// </summary>
    private int CalcularIndice(float porcentagem, int totalFrames)
    {
        porcentagem = Mathf.Clamp01(porcentagem);
        int idx = Mathf.FloorToInt((1f - porcentagem) * (totalFrames - 1));
        return Mathf.Clamp(idx, 0, totalFrames - 1);
    }

    /// <summary>
    /// Forçar atualização visual imediata (chame após respawn, etc.)
    /// </summary>
    public void ForcarAtualizacao()
    {
        indiceVidaAtual = -1;
        indiceStaminaAtual = -1;
        AtualizarHUD();
    }
}

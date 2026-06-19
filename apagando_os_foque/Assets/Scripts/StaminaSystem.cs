using UnityEngine;
using UnityEngine.Events;

public class StaminaSystem : MonoBehaviour
{
    [Header("Stamina")]
    public float staminaMax = 100f;
    public float staminaAtual;
    public float drenagemStamina = 15f;
    public float regeneracaoStamina = 10f;

    [Header("Vida")]
    public float vidaMax = 100f;
    public float vidaAtual;

    [Header("Dano progressivo na luz")]
    public float danoPorSegundo = 5f;
    public float multiplicadorDano = 1.2f;
    private float tempoDanando = 0f;
    private float intensidadeLuzAtual = 1f;

    [Header("Eventos")]
    public UnityEvent aoMorrer;
    public UnityEvent<float> aoAlterarStamina;
    public UnityEvent<float> aoAlterarVida;

    private bool naLuz = false;

    void Start()
    {
        staminaAtual = staminaMax;
        vidaAtual = vidaMax;
    }

    void Update()
    {
        if (naLuz)
            ProcessarDanoLuz();
        else
            ProcessarRecuperacao();
    }

    void ProcessarDanoLuz()
    {
        tempoDanando += Time.deltaTime;

        if (staminaAtual > 0)
        {
            staminaAtual -= drenagemStamina * intensidadeLuzAtual * Time.deltaTime;
            staminaAtual = Mathf.Max(staminaAtual, 0);
            aoAlterarStamina?.Invoke(staminaAtual / staminaMax);
        }
        else
        {
            float danoFinal = danoPorSegundo * intensidadeLuzAtual * Mathf.Pow(multiplicadorDano, tempoDanando);
            vidaAtual -= danoFinal * Time.deltaTime;
            vidaAtual = Mathf.Max(vidaAtual, 0);
            aoAlterarVida?.Invoke(vidaAtual / vidaMax);

            if (vidaAtual <= 0)
            {
                aoMorrer?.Invoke();
                GameManager.Instance?.Morrer();
                enabled = false;
            }
        }
    }

    void ProcessarRecuperacao()
    {
        tempoDanando = 0f;

        if (staminaAtual < staminaMax)
        {
            staminaAtual += regeneracaoStamina * Time.deltaTime;
            staminaAtual = Mathf.Min(staminaAtual, staminaMax);
            aoAlterarStamina?.Invoke(staminaAtual / staminaMax);
        }
    }

    public void EntrarNaLuz() => naLuz = true;
    public void SairDaLuz()
    {
        naLuz = false;
        tempoDanando = 0f;
    }

    public void SetIntensidadeLuz(float intensidade) =>
        intensidadeLuzAtual = Mathf.Clamp01(intensidade);

    public float GetVidaNormalizada() => vidaAtual / vidaMax;
    public float GetStaminaNormalizada() => staminaAtual / staminaMax;
}

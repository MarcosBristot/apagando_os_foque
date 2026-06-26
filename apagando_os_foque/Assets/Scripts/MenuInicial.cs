using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Tela de início do Adelar Bombachudo.
/// Hierarquia esperada no Canvas:
///
///   Canvas
///   └── PainelMenuInicial
///       ├── ImagemFundo      (Image — sua arte gerada por IA, Full Screen)
///       ├── Logo             (Image — logo do jogo por cima da arte)
///       └── BotaoJogar       (Button)
///
/// Coloque este script no GameObject PainelMenuInicial.
/// No Inspector arraste os campos e defina o nome da cena do jogo.
/// </summary>
public class MenuInicial : MonoBehaviour
{
    [Header("Referências UI")]
    public Image imagemFundo;
    public Image logo;
    public Button botaoJogar;

    [Header("Configuração")]
    [Tooltip("Nome exato da cena do jogo no Build Settings")]
    public string nomeCenaJogo = "Cena Raul - 23-06 - principal";

    [Tooltip("Duração do fade de entrada em segundos")]
    public float tempoFadeEntrada = 1.2f;

    [Tooltip("Duração do fade de saída ao clicar Jogar")]
    public float tempoFadeSaida = 0.8f;

    // Grupo de canvas para fade geral
    private CanvasGroup canvasGroup;

    void Awake()
    {
        // Cria ou obtém o CanvasGroup para o fade
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Começa invisível
        canvasGroup.alpha = 0f;
    }

    void Start()
    {
        // Conecta o botão
        if (botaoJogar != null)
            botaoJogar.onClick.AddListener(OnJogarClicado);

        // Fade de entrada
        StartCoroutine(FadeIn());

        // Pausa o jogo enquanto no menu (caso venha de uma cena compartilhada)
        Time.timeScale = 1f;
    }

    void OnJogarClicado()
    {
        botaoJogar.interactable = false;
        StartCoroutine(FadeESair());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < tempoFadeEntrada)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / tempoFadeEntrada);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeESair()
    {
        float t = 0f;
        while (t < tempoFadeSaida)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / tempoFadeSaida);
            yield return null;
        }
        SceneManager.LoadScene(nomeCenaJogo);
    }

    // Atalho de teclado: Enter ou Space também inicia
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            if (botaoJogar != null && botaoJogar.interactable)
                OnJogarClicado();
    }
}

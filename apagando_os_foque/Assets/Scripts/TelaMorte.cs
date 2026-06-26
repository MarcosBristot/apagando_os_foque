using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

/// <summary>
/// Tela de morte do Adelar Bombachudo.
/// Hierarquia esperada no Canvas:
///
///   Canvas
///   └── PainelMorte           ← este script aqui, começa DESATIVADO
///       ├── ImagemFundo       (Image — arte gerada por IA, Full Screen)
///       ├── TxtTitulo         (TextMeshProUGUI — ex: "ADELAR CAIU...")
///       ├── TxtTempSobreviveu (TextMeshProUGUI — ex: "Sobreviveu: 1m 23s")
///       ├── BotaoReiniciar    (Button)
///       └── BotaoMenu         (Button)
///
/// Coloque este script no PainelMorte.
/// O GameManager chama Mostrar() via evento aoMorrer.
/// </summary>
public class TelaMorte : MonoBehaviour
{
    [Header("Referências UI")]
    public TextMeshProUGUI txtTitulo;
    public TextMeshProUGUI txtTempoSobreviveu;
    public Button botaoReiniciar;
    public Button botaoMenu;

    [Header("Configuração")]
    [Tooltip("Nome exato da cena do jogo para reiniciar")]
    public string nomeCenaJogo = "Cena Raul - 23-06 - principal";

    [Tooltip("Nome exato da cena do menu inicial")]
    public string nomeCenaMenu = "MenuInicial";

    [Tooltip("Mensagens aleatórias de morte — pixel art style")]
    public string[] mensagensMorte = new string[]
    {
        "ADELAR CAIU...",
        "A LUZ TE ACHOU.",
        "GAME OVER, BOMBACHUDO.",
        "A SOMBRA SUMIU.",
        "VOCÊ NÃO ERA SOMBRA SUFICIENTE."
    };

    [Tooltip("Duração do fade de entrada")]
    public float tempoFade = 1.0f;

    // Controle de tempo
    private float tempoInicioCena;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Começa oculto
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }

    void Start()
    {
        // Marca início para calcular tempo depois
        tempoInicioCena = Time.time;

        // Conecta botões
        if (botaoReiniciar != null)
            botaoReiniciar.onClick.AddListener(Reiniciar);

        if (botaoMenu != null)
            botaoMenu.onClick.AddListener(IrParaMenu);
    }

    /// <summary>
    /// Chame este método pelo evento aoMorrer do GameManager.
    /// </summary>
    public void Mostrar()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f; // Pausa o jogo

        // Título aleatório
        if (txtTitulo != null && mensagensMorte.Length > 0)
            txtTitulo.text = mensagensMorte[Random.Range(0, mensagensMorte.Length)];

        // Tempo sobrevivido
        if (txtTempoSobreviveu != null)
        {
            float segundos = Time.time - tempoInicioCena;
            int min = Mathf.FloorToInt(segundos / 60f);
            int seg = Mathf.FloorToInt(segundos % 60f);
            txtTempoSobreviveu.text = $"Sobreviveu: {min:00}:{seg:00}";
        }

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < tempoFade)
        {
            t += Time.unscaledDeltaTime; // unscaled porque Time.timeScale = 0
            canvasGroup.alpha = Mathf.Clamp01(t / tempoFade);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeCenaJogo);
    }

    void IrParaMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeCenaMenu);
    }

    // R para reiniciar, Escape para menu (atalhos)
    void Update()
    {
        if (canvasGroup.interactable)
        {
            if (Input.GetKeyDown(KeyCode.R)) Reiniciar();
            if (Input.GetKeyDown(KeyCode.Escape)) IrParaMenu();
        }
    }
}

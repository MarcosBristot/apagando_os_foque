using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    [Header("Telas")]
    public GameObject telaMorte;
    public GameObject telaVitoria;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (telaMorte != null)  telaMorte.SetActive(false);
        if (telaVitoria != null) telaVitoria.SetActive(false);

        // conecta eventos do GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.aoMorrer.AddListener(MostrarTelaMorte);
            GameManager.Instance.aoVencer.AddListener(MostrarTelaVitoria);
        }
    }

    public void MostrarTelaMorte()
    {
        if (telaMorte != null)
            telaMorte.SetActive(true);
    }

    public void MostrarTelaVitoria()
    {
        if (telaVitoria != null)
            telaVitoria.SetActive(true);
    }

    public void ReiniciarFase()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CarregarCena(string nomeCena)
    {
        SceneManager.LoadScene(nomeCena);
    }
}

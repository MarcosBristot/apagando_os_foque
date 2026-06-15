using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum EstadoJogo { Jogando, Morto, Vitoria }
    public EstadoJogo estadoAtual = EstadoJogo.Jogando;

    [Header("Eventos")]
    public UnityEvent aoMorrer;
    public UnityEvent aoVencer;
    public UnityEvent aoReiniciar;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Morrer()
    {
        if (estadoAtual != EstadoJogo.Jogando) return;
        estadoAtual = EstadoJogo.Morto;
        Debug.Log("GAME OVER");
        aoMorrer?.Invoke();
    }

    public void Vencer()
    {
        if (estadoAtual != EstadoJogo.Jogando) return;
        estadoAtual = EstadoJogo.Vitoria;
        Debug.Log("VITÓRIA!");
        aoVencer?.Invoke();
    }

    public void Reiniciar()
    {
        estadoAtual = EstadoJogo.Jogando;
        aoReiniciar?.Invoke();
        GameSceneManager.Instance?.ReiniciarFase();
    }

    public bool EstaJogando() => estadoAtual == EstadoJogo.Jogando;
}

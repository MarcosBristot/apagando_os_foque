using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

public class AchievementSystem : MonoBehaviour
{
    public static AchievementSystem Instance;

    [Header("Conquistas")]
    public Achievement[] conquistas;

    [Header("UI de notificação")]
    public GameObject painelNotificacao;
    public Text textoNotificacao;
    public float duracaoNotificacao = 3f;

    public UnityEvent<Achievement> aoConquistarAlgo;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (painelNotificacao != null)
            painelNotificacao.SetActive(false);
    }

    public void Conquistar(string nomeConquista)
    {
        foreach (var c in conquistas)
        {
            if (c.nomeConquista == nomeConquista && !c.concluida)
            {
                c.concluida = true;
                aoConquistarAlgo?.Invoke(c);
                StartCoroutine(MostrarNotificacao(c));
                Debug.Log($"Conquista desbloqueada: {c.nomeConquista}");
                return;
            }
        }
    }

    IEnumerator MostrarNotificacao(Achievement conquista)
    {
        if (painelNotificacao == null) yield break;

        if (textoNotificacao != null)
            textoNotificacao.text = $"🏆 {conquista.nomeConquista}\n{conquista.descricao}";

        painelNotificacao.SetActive(true);
        yield return new WaitForSeconds(duracaoNotificacao);
        painelNotificacao.SetActive(false);
    }
}

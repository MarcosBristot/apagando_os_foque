using UnityEngine;
using UnityEngine.Events;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance;

    [Header("Eventos")]
    public UnityEvent aoApagarTodasAsLuzes;

    private int totalLuzes = 0;
    private int luzesApagadas = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // conta apenas pelos GameObjects com tag IlluminatedZone
        totalLuzes = GameObject.FindGameObjectsWithTag("IlluminatedZone").Length;
        Debug.Log($"Total de luzes na cena: {totalLuzes}");
    }

    public void RegistrarLuzApagada()
    {
        luzesApagadas++;
        Debug.Log($"Luzes apagadas: {luzesApagadas}/{totalLuzes}");

        if (luzesApagadas >= totalLuzes)
            aoApagarTodasAsLuzes?.Invoke();
    }
}
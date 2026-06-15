using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Arma
{
    public string nome;
    public Sprite icone;
    public float dano;
    public float cooldown;
}

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Header("Armas")]
    public List<Arma> armas = new List<Arma>();
    public int slotAtivo = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Arma GetArmaAtiva()
    {
        if (armas.Count == 0) return null;
        return armas[slotAtivo];
    }

    public void TrocarArma(int slot)
    {
        if (slot >= 0 && slot < armas.Count)
        {
            slotAtivo = slot;
            Debug.Log($"Arma equipada: {armas[slotAtivo].nome}");
        }
    }

    public void AdicionarArma(Arma arma)
    {
        armas.Add(arma);
        Debug.Log($"Arma adicionada ao inventário: {arma.nome}");
    }
}

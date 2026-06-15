using UnityEngine;

[CreateAssetMenu(fileName = "Conquista", menuName = "Jogo/Conquista")]
public class Achievement : ScriptableObject
{
    public string nomeConquista;
    public string descricao;
    public Sprite icone;
    public bool concluida = false;
}

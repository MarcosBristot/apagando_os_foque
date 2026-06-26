// ─── SpriteSheetHelper.cs ───────────────────────────────────────────────────
// Script de EDITOR que facilita o fatiamento dos sprite sheets no Unity.
// Coloque este arquivo em Assets/Editor/ (pasta separada).
// ────────────────────────────────────────────────────────────────────────────
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Ferramenta de editor para fatiar automaticamente os sprite sheets
/// das barras de vida e stamina.
///
/// Como usar:
/// 1. Selecione o sprite sheet no Project
/// 2. Menu: Tools → Adelar → Fatiar Sprite Sheet de Barra
/// 3. Configure linhas/colunas e clique em Fatiar
/// </summary>
public class SpriteSheetHelper : EditorWindow
{
    private Texture2D textura;
    private int colunas = 3;
    private int linhas = 4;
    private string nomeBase = "frame";

    [MenuItem("Tools/Adelar/Fatiar Sprite Sheet de Barra")]
    public static void AbrirJanela()
    {
        GetWindow<SpriteSheetHelper>("Fatiar Sprite Sheet");
    }

    private void OnGUI()
    {
        GUILayout.Label("Configuração do Sprite Sheet", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        textura = (Texture2D)EditorGUILayout.ObjectField("Sprite Sheet", textura, typeof(Texture2D), false);
        colunas = EditorGUILayout.IntField("Colunas", colunas);
        linhas = EditorGUILayout.IntField("Linhas", linhas);
        nomeBase = EditorGUILayout.TextField("Nome base dos frames", nomeBase);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            $"Vai criar {colunas * linhas} sprites.\n" +
            "Barra de Vida (blood): 3 colunas × 5 linhas = 13 frames\n" +
            "Bateria (stamina): 3 colunas × 4 linhas = 12 frames\n\n" +
            "Certifique-se de que o Texture Type está como 'Sprite (2D and UI)'\n" +
            "e Sprite Mode está como 'Multiple'.",
            MessageType.Info
        );

        EditorGUILayout.Space();

        if (GUILayout.Button("Fatiar Automaticamente") && textura != null)
        {
            FatiarSpriteSheet();
        }

        if (GUILayout.Button("Copiar Array de Sprites para Console") && textura != null)
        {
            MostrarOrdemSprites();
        }
    }

    private void FatiarSpriteSheet()
    {
        string caminho = AssetDatabase.GetAssetPath(textura);
        TextureImporter importer = AssetImporter.GetAtPath(caminho) as TextureImporter;

        if (importer == null)
        {
            Debug.LogError("Não foi possível obter o TextureImporter.");
            return;
        }

        // Configura o importer
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point; // Pixel art: sem blur
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        int larguraFrame = textura.width / colunas;
        int alturaFrame = textura.height / linhas;

        List<SpriteMetaData> sprites = new List<SpriteMetaData>();
        int contador = 0;

        // Lê de cima para baixo, esquerda para direita
        for (int linha = linhas - 1; linha >= 0; linha--)
        {
            for (int col = 0; col < colunas; col++)
            {
                SpriteMetaData meta = new SpriteMetaData
                {
                    name = $"{nomeBase}_{contador:D2}",
                    rect = new Rect(
                        col * larguraFrame,
                        linha * alturaFrame,
                        larguraFrame,
                        alturaFrame
                    ),
                    pivot = new Vector2(0.5f, 0.5f),
                    alignment = (int)SpriteAlignment.Center
                };
                sprites.Add(meta);
                contador++;
            }
        }

        importer.spritesheet = sprites.ToArray();
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        Debug.Log($"[SpriteSheetHelper] Fatiamento concluído! {contador} sprites criados em: {caminho}");
    }

    private void MostrarOrdemSprites()
    {
        string caminho = AssetDatabase.GetAssetPath(textura);
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(caminho);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"Sprites em {textura.name}:");

        foreach (var obj in sprites)
        {
            if (obj is Sprite sprite)
                sb.AppendLine($"  - {sprite.name}");
        }

        Debug.Log(sb.ToString());
    }
}
#endif

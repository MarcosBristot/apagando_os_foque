using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Referências")]
    public Slider barraStamina;
    public Slider barraVida;
    public StaminaSystem stamina;

    void Update()
    {
        if (stamina == null) return;
        barraStamina.value = stamina.GetStaminaNormalizada();
        barraVida.value    = stamina.GetVidaNormalizada();
    }
}

using UnityEngine;

public class Lantern : MonoBehaviour
{
    private LightSource lightSource;
    private LampiaoCaipira lampiaoCaipira;
    private PosteLight posteLight;

    void Awake()
    {
        lightSource = GetComponent<LightSource>();
        lampiaoCaipira = GetComponent<LampiaoCaipira>();
        posteLight = GetComponent<PosteLight>();
    }

    public void TakeHit()
    {
        if (lightSource != null && lampiaoCaipira == null && posteLight == null)
            lightSource.BreakLight();

        if (lampiaoCaipira != null)
            lampiaoCaipira.BreakLight();

        if (posteLight != null)
            posteLight.BreakLight();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
            TakeHit();
    }
}
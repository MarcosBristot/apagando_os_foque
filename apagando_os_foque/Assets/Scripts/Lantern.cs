using UnityEngine;

public class Lantern : MonoBehaviour
{
    private LightSource lightSource;
    private LampiaoCaipira lampiaoCaipira;

    void Awake()
    {
        lightSource = GetComponent<LightSource>();
        lampiaoCaipira = GetComponent<LampiaoCaipira>();
    }

    public void TakeHit()
    {
        if (lightSource != null) lightSource.BreakLight();
        if (lampiaoCaipira != null) lampiaoCaipira.BreakLight();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
            TakeHit();
    }
}
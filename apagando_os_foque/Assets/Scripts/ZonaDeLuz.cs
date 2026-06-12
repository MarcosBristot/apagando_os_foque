using UnityEngine;

public class ZonaDeLuz : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("Player"))
            outro.GetComponent<PlayerController>().EntrarNaLuz();
    }

    void OnTriggerExit2D(Collider2D outro)
    {
        if (outro.CompareTag("Player"))
            outro.GetComponent<PlayerController>().SairDaLuz();
    }
}
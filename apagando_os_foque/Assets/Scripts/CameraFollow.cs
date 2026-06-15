using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform alvo;
    public float velocidadeCamera = 5f;

    void LateUpdate()
    {
        if (alvo == null) return;
        Vector3 posicaoAlvo = new Vector3(alvo.position.x, alvo.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, posicaoAlvo, velocidadeCamera * Time.deltaTime);
    }
}

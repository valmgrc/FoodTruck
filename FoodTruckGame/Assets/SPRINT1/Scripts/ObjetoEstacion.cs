using UnityEngine;

public class ObjetoEstacion : MonoBehaviour
{
    public Vector3 posicionInicial;
    public Quaternion rotacionInicial;
    public Transform padreInicial;

    void Awake()
    {
   
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
        padreInicial = transform.parent;
    }

    public void ResetearPosicionOriginal()
    {
        transform.SetParent(padreInicial);
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }
}
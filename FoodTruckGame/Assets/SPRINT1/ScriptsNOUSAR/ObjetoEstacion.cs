using UnityEngine;

public class ObjetoEstacion : MonoBehaviour
{
    [HideInInspector] public Vector3 posicionInicial;
    [HideInInspector] public Quaternion rotacionInicial;
    [HideInInspector] public Transform padreInicial;

    private Rigidbody rb;
    private bool posicionGuardada = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GuardarPosicionInicial();
    }

    public void GuardarPosicionInicial()
    {
        if (!posicionGuardada)
        {
            posicionInicial = transform.position;
            rotacionInicial = transform.rotation;
            padreInicial = transform.parent;
            posicionGuardada = true;
        }
    }

    public void ResetearPosicionOriginal()
    {
        transform.SetParent(padreInicial);
        transform.position = posicionInicial + new Vector3(0f, 0.04f, 0f);
        transform.rotation = rotacionInicial;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }
}
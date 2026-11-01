using UnityEngine;

public class CamaraMenu : MonoBehaviour
{
    public float velocidadDePaneo = 5f;

    void Update()
    {
        transform.Rotate(Vector3.up * velocidadDePaneo * Time.deltaTime);
    }
}

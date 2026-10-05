using UnityEngine;

public class Cebollin_Cortar : MonoBehaviour
{
    public GameObject cebollinEntero;
    public GameObject cebollinMitad;
    public GameObject cebollinPicado;

    private int estadoCorte = 0;
    public int EstadoCorte => estadoCorte;

    private bool puedeRecibirCorte = true;

    void Start()
    {
        ActualizarVisuales();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (puedeRecibirCorte && (other.CompareTag("Cuchillo") || other.name.ToLower().Contains("cuchillo")))
        {
            AvanzarCorte();
        }
    }

    public void AvanzarCorte()
    {
        if (estadoCorte < 2)
        {
            estadoCorte++;
            puedeRecibirCorte = false; 
            Invoke(nameof(ResetearCorte), 0.3f); 

            ActualizarVisuales();
        }
    }

    void ResetearCorte()
    {
        puedeRecibirCorte = true;
    }

    public void ActualizarVisuales()
    {
        if (cebollinEntero != null) cebollinEntero.SetActive(estadoCorte == 0);
        if (cebollinMitad != null) cebollinMitad.SetActive(estadoCorte == 1);
        if (cebollinPicado != null) cebollinPicado.SetActive(estadoCorte == 2);
    }
}
using UnityEngine;

public class Daikon_Cortar : MonoBehaviour
{
    public GameObject daikonEntero;
    public GameObject daikonMitad;
    public GameObject daikonPicado;

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
        if (daikonEntero != null) daikonEntero.SetActive(estadoCorte == 0);
        if (daikonMitad != null) daikonMitad.SetActive(estadoCorte == 1);
        if (daikonPicado != null) daikonPicado.SetActive(estadoCorte == 2);
    }
}
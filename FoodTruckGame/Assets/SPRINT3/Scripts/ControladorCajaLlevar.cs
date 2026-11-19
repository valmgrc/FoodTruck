using UnityEngine;

public class ControladorCajaLlevar : MonoBehaviour
{
    //public GameObject planoSopaMitad; 
    public GameObject planoSopaLleno; 

    public int porcionesServidas = 0; 
    public bool estaLleno => porcionesServidas >= 1;

    void Start()
    {
        ActualizarVisuales();
    }

    void OnMouseDown()
    {
        CucharonSystem cucharon = FindObjectOfType<CucharonSystem>();

        if (cucharon != null && cucharon.tieneSopa && !estaLleno)
        {
            porcionesServidas++;
            cucharon.VaciarEnVasija();
            ActualizarVisuales();
        }
    }

    public void ActualizarVisuales()
    {
        if (planoSopaLleno != null) planoSopaLleno.SetActive(porcionesServidas >= 1);
    }
}
using UnityEngine;

public class VasijaAlmacenamiento : MonoBehaviour
{
    public int porcionesActuales = 0;
    public int porcionesMaximas = 2;
    public GameObject[] nivelesDeLlenado;

    public int porcionesCucharadasRestantes = 20; 

    void Start()
    {
        ActualizarVisualesVasija();
    }

    void OnMouseDown()
    {
        CucharonSystem cucharon = FindObjectOfType<CucharonSystem>();

        if (cucharon != null)
        {
            if (cucharon.tieneSopa)
            {
                bool exito = AgregarPorcion();

                if (exito)
                {
                    cucharon.VaciarEnVasija();

                    if (TutorialManager.Instance != null)
                    {
                        TutorialManager.Instance.NotificarSopaServida();
                    }
                }
            }
            else if (!cucharon.tieneSopa && porcionesActuales > 0 && porcionesCucharadasRestantes > 0)
            {
                cucharon.TomarSopaDeVasija();
                porcionesCucharadasRestantes--;

                if (porcionesCucharadasRestantes <= 0)
                {
                    porcionesActuales = 0;
                    ActualizarVisualesVasija();
                }
            }
        }
    }

    public bool AgregarPorcion()
    {
        if (porcionesActuales >= porcionesMaximas)
        {
            return false;
        }

        porcionesActuales++;
        porcionesCucharadasRestantes = 20; 
        ActualizarVisualesVasija();
        return true;
    }

    void ActualizarVisualesVasija()
    {
        for (int i = 0; i < nivelesDeLlenado.Length; i++)
        {
            if (nivelesDeLlenado[i] != null)
            {
                nivelesDeLlenado[i].SetActive(i < porcionesActuales);
            }
        }
    }
}
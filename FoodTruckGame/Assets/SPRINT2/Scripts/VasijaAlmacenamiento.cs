using UnityEngine;

public class VasijaAlmacenamiento : MonoBehaviour
{
    public int porcionesActuales = 0;
    public int porcionesMaximas = 2;

    public GameObject[] nivelesDeLlenado;

    void Start()
    {
        ActualizarVisualesVasija();
    }

    void OnMouseDown()
    {
        CucharonSystem cucharon = FindObjectOfType<CucharonSystem>();

        if (cucharon != null && cucharon.tieneSopa)
        {
            bool exito = AgregarPorcion();

            if (exito)
            {
                cucharon.VaciarEnVasija();

                if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.NotificarSopaServida();
                }

                if (porcionesActuales >= porcionesMaximas)
                {
                   
                }
            }
        }
        else
        {

        }
    }

    public bool AgregarPorcion()
    {
        if (porcionesActuales >= porcionesMaximas)
        {
            return false; 
        }

        porcionesActuales++;
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
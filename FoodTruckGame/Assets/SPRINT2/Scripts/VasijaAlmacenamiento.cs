using UnityEngine;

public class VasijaAlmacenamiento : MonoBehaviour
{
    public int porcionesActuales = 0;
    public int porcionesMaximas = 2;

    public GameObject[] nivelesDeLlenado;

    [Header("Servicio en Fase de Pedidos")]
    public int porcionesCucharadasRestantes = 20; // 20 cucharadas antes de vaciarse

    void Start()
    {
        ActualizarVisualesVasija();
    }

    void OnMouseDown()
    {
        CucharonSystem cucharon = FindObjectOfType<CucharonSystem>();

        if (cucharon != null)
        {
            // FASE 1: Llenar la vasija desde la olla (cucharón cargado)
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
            // FASE 2: Servir de la vasija al Bowl (cucharón vacío)
            else if (!cucharon.tieneSopa && porcionesActuales > 0 && porcionesCucharadasRestantes > 0)
            {
                cucharon.TomarSopaDeVasija();
                porcionesCucharadasRestantes--;

                // Si se agotan las 20 cucharadas, la vasija queda vacía
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
        porcionesCucharadasRestantes = 20; // Se recarga a 20 porciones al llenarla
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
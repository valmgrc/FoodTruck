using UnityEngine;
using System.Collections;
using TMPro;

public class SistemaCorteObjeto : MonoBehaviour
{
    public int clicsNecesarios = 3;
    private int clicsActuales = 0;

    public GameObject objetoEntero;
    public GameObject prefabPicado;

    public static bool cuchilloEnMano = false;

    public GameObject contenedorVacios;   
    public GameObject contenedorRellenos; 
    public GameObject[] circulosRellenos; 

    public TextMeshProUGUI textoAviso;   
    public float tiempoVisibilidadTexto = 1.5f;
    public float tiempoFade = 0.8f;

    void Start()
    {
        ResetearUI();
    }

    void ResetearUI()
    {
        if (contenedorVacios != null) contenedorVacios.SetActive(false);
        if (contenedorRellenos != null) contenedorRellenos.SetActive(false);

        foreach (var circulo in circulosRellenos)
        {
            if (circulo != null) circulo.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        if (!cuchilloEnMano)
        {
            return;
        }

        if (clicsActuales >= clicsNecesarios) return;

        if (clicsActuales == 0)
        {
            ResetearUI();

            if (contenedorVacios != null) contenedorVacios.SetActive(true);
            if (contenedorRellenos != null) contenedorRellenos.SetActive(true);
        }

        clicsActuales++;

      
        int indiceCirculo = clicsActuales - 1;
        if (indiceCirculo < circulosRellenos.Length && circulosRellenos[indiceCirculo] != null)
        {
            circulosRellenos[indiceCirculo].SetActive(true);
        }

        if (clicsActuales >= clicsNecesarios)
        {
            if (prefabPicado != null && objetoEntero != null)
            {
                Instantiate(prefabPicado, objetoEntero.transform.position, objetoEntero.transform.rotation);
            }

            ResetearUI();

            if (textoAviso != null)
            {
                TextoUICorte.Instancia.MostrarAviso(textoAviso, "Verdura cortada", tiempoVisibilidadTexto, tiempoFade);
            }
            if (objetoEntero != null)
            {
                Destroy(objetoEntero);
            }

        }
    }
}
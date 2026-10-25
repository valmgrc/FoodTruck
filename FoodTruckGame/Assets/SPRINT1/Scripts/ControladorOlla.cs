using UnityEngine;
using System.Collections;
using TMPro;

public class ControladorOlla : MonoBehaviour
{
    public int cantidadDaikon = 0;
    public int cantidadCebollin = 0;
    public int cantidadMijo = 0;

    public int porcionesDisponiblesEnOlla = 0;
    public int porcionesCocinandose = 0;
    public int maxPorcionesPermitidas = 2;

    public float tiempoCoccion = 15f;
    private float tiempoRestante = 0f;
    public bool estaCocinando = false;
    public bool estaEnParrilla = false; 

    public TextMeshProUGUI textoContador;
    public GameObject objetoAguaAzul;
    public GameObject objetoAguaNaranja;

    private ObjectSelected miObjectSelected;

    void Start()
    {
        miObjectSelected = GetComponent<ObjectSelected>();

        if (miObjectSelected != null)
        {
            miObjectSelected.puedoInteractuar = true;
        }

        if (objetoAguaAzul != null) objetoAguaAzul.SetActive(true);
        if (objetoAguaNaranja != null) objetoAguaNaranja.SetActive(false);
        if (textoContador != null) textoContador.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Parrilla") || other.name.Contains("Parrilla") || other.name.Contains("Estufa"))
        {
            estaEnParrilla = true;
            return;
        }

        IngredienteCocinable ingrediente = other.GetComponent<IngredienteCocinable>();

        if (ingrediente != null)
        {
            int porcionesTotalesOcupadas = porcionesDisponiblesEnOlla + porcionesCocinandose;

            if (porcionesTotalesOcupadas >= maxPorcionesPermitidas)
            {
                return;
            }

            if (ingrediente.tipo == IngredienteCocinable.TipoIngrediente.Daikon)
            {
                cantidadDaikon++;
                ingrediente.EntrarAOlla();
            }
            else if (ingrediente.tipo == IngredienteCocinable.TipoIngrediente.Cebollin)
            {
                cantidadCebollin++;
                ingrediente.EntrarAOlla();
            }
            else if (ingrediente.tipo == IngredienteCocinable.TipoIngrediente.Mijo)
            {
                cantidadMijo++;
                ingrediente.EntrarAOlla();
            }

            RevisarYConvertirAPorciones();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Parrilla") || other.name.Contains("Parrilla") || other.name.Contains("Estufa"))
        {
            estaEnParrilla = false;
        }
    }

    void RevisarYConvertirAPorciones()
    {
        bool seFormoNuevaPorcion = false;

        while (cantidadDaikon >= 1 && cantidadCebollin >= 1 && cantidadMijo >= 1 && (porcionesDisponiblesEnOlla + porcionesCocinandose) < maxPorcionesPermitidas)
        {
            cantidadDaikon -= 1;
            cantidadCebollin -= 1;
            cantidadMijo -= 1;

            porcionesCocinandose++;
            seFormoNuevaPorcion = true;
        }

        if (seFormoNuevaPorcion)
        {
            tiempoRestante = tiempoCoccion;

            if (!estaCocinando)
            {
                StartCoroutine(RutinaCoccion());
            }
            if (seFormoNuevaPorcion)
            {
                tiempoRestante = tiempoCoccion;


                if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.NotificarIngredientesEnOlla();
                }

                if (!estaCocinando)
                {
                    StartCoroutine(RutinaCoccion());
                }
            }
        }
    }

    IEnumerator RutinaCoccion()
    {
        estaCocinando = true;

        if (objetoAguaAzul != null) objetoAguaAzul.SetActive(false);
        if (objetoAguaNaranja != null && porcionesDisponiblesEnOlla <= 0)
        {
            objetoAguaNaranja.SetActive(false);
        }

        if (textoContador != null) textoContador.gameObject.SetActive(true);

        while (tiempoRestante > 0f)
        {
            if (estaEnParrilla)
            {
                tiempoRestante -= Time.deltaTime;

                if (textoContador != null)
                {
                    textoContador.text = Mathf.Ceil(tiempoRestante).ToString() + "s";
                }
            }
            else
            {
                if (textoContador != null)
                {
                    textoContador.text = "Poner a hervir";
                }
            }

            yield return null;
        }

        estaCocinando = false;

        porcionesDisponiblesEnOlla += porcionesCocinandose;
        porcionesCocinandose = 0;

        if (textoContador != null)
        {
            textoContador.text = "Listo";
        }

        if (objetoAguaNaranja != null) objetoAguaNaranja.SetActive(true);

        if (TutorialManager.Instance != null)
        {
           TutorialManager.Instance.NotificarSopaLista();
        }
    }

    void OnMouseDown()
    {
        if (objetoAguaNaranja != null && objetoAguaNaranja.activeSelf && !estaCocinando)
        {
            CucharonSystem cucharon = FindObjectOfType<CucharonSystem>();

            if (cucharon != null)
            {
                cucharon.TomarSopaDeOlla(this);
            }
        }
    }

    public bool ExtraerPorcionDeOlla()
    {
        if (porcionesDisponiblesEnOlla > 0 && !estaCocinando)
        {
            porcionesDisponiblesEnOlla--;

            if (porcionesDisponiblesEnOlla <= 0 && porcionesCocinandose <= 0)
            {
                ReiniciarOlla();
            }
            return true;
        }
        return false;
    }

    void ReiniciarOlla()
    {
        estaCocinando = false;
        if (objetoAguaAzul != null) objetoAguaAzul.SetActive(true);
        if (objetoAguaNaranja != null) objetoAguaNaranja.SetActive(false);
        if (textoContador != null) textoContador.gameObject.SetActive(false);
    }
}
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ControladorOlla : MonoBehaviour
{
    public int cantidadDaikon = 0;
    public int cantidadCebollin = 0;
    public int cantidadMijo = 0;

    public int porcionesDisponiblesEnOlla = 0;
    public int porcionesCocinandose = 0;
    public int maxPorcionesPermitidas = 2;

    public bool estaCocinando = false;
    public bool estaEnParrilla = false;


    public GameObject panelMinijuegoContainer;
    public Slider sliderProgresoMinijuego;
    public GameObject iconoFlechaGuia;
    public float girosNecesarios = 15f;
    private float progresoActual = 0f;


    public GameObject objetoAguaAzul;
    public GameObject objetoAguaNaranja;
    public Transform puntoCentroOlla;

    private ObjectSelected miObjectSelected;
    private bool estaSiendoMirado = false; 

    void Start()
    {
        miObjectSelected = GetComponent<ObjectSelected>();

        if (miObjectSelected != null)
        {
            miObjectSelected.puedoInteractuar = true;
        }

        if (objetoAguaAzul != null) objetoAguaAzul.SetActive(true);
        if (objetoAguaNaranja != null) objetoAguaNaranja.SetActive(false);

        if (panelMinijuegoContainer != null) panelMinijuegoContainer.SetActive(false);
    }
    private void OnMouseEnter()
    {
        estaSiendoMirado = true;
    }

    private void OnMouseExit()
    {
        estaSiendoMirado = false;
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
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.NotificarIngredientesEnOlla();
            }

            if (!estaCocinando)
            {
                StartCoroutine(RutinaMinijuegoRevolver());
            }
        }
    }

    IEnumerator RutinaMinijuegoRevolver()
    {
        estaCocinando = true;
        progresoActual = 0f;

        if (objetoAguaAzul != null) objetoAguaAzul.SetActive(false);
        if (objetoAguaNaranja != null && porcionesDisponiblesEnOlla <= 0)
        {
            objetoAguaNaranja.SetActive(false);
        }

        if (panelMinijuegoContainer != null) panelMinijuegoContainer.SetActive(true);
        if (sliderProgresoMinijuego != null)
        {
            sliderProgresoMinijuego.minValue = 0f;
            sliderProgresoMinijuego.maxValue = girosNecesarios;
            sliderProgresoMinijuego.value = 0f;
        }
        if (iconoFlechaGuia != null) iconoFlechaGuia.SetActive(true);

        bool primerGiroHecho = false;

        while (progresoActual < girosNecesarios)
        {

            if (estaEnRangoOlla() && CucharonSystem.cucharonEnMano && estaSiendoMirado && Input.GetMouseButton(0))
            {
                float deltaMouseX = Input.GetAxis("Mouse X");
                float deltaMouseY = Input.GetAxis("Mouse Y");
                float movimientoMagnitud = Mathf.Abs(deltaMouseX) + Mathf.Abs(deltaMouseY);

                if (movimientoMagnitud > 0.1f)
                {
                    progresoActual += movimientoMagnitud * Time.deltaTime * 5f;
                    if (sliderProgresoMinijuego != null) sliderProgresoMinijuego.value = progresoActual;

                    if (!primerGiroHecho)
                    {
                        primerGiroHecho = true;
                        if (iconoFlechaGuia != null) iconoFlechaGuia.SetActive(false);
                    }

                    CucharonSystem cucharon = FindObjectOfType<CucharonSystem>();
                    if (cucharon != null && puntoCentroOlla != null)
                    {
                        cucharon.transform.RotateAround(puntoCentroOlla.position, Vector3.up, 300f * Time.deltaTime);
                    }
                }
            }

            yield return null;
        }

        estaCocinando = false;

        if (panelMinijuegoContainer != null) panelMinijuegoContainer.SetActive(false);

        porcionesDisponiblesEnOlla += porcionesCocinandose;
        porcionesCocinandose = 0;

        if (objetoAguaNaranja != null) objetoAguaNaranja.SetActive(true);

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.NotificarSopaLista();
        }
    }

    bool estaEnRangoOlla()
    {
        return estaEnParrilla;
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
        if (panelMinijuegoContainer != null) panelMinijuegoContainer.SetActive(false);
    }
}
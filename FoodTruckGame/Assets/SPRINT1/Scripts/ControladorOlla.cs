using UnityEngine;
using System.Collections;
using TMPro;

public class ControladorOlla : MonoBehaviour
{

    public bool tieneDaikon = false;
    public bool tieneCebollin = false;
    public bool tieneMijo = false;


    public float tiempoCoccion = 15f;
    private bool estaCocinando = false;

    public TextMeshProUGUI textoContador;
    public GameObject objetoAguaAzul;
    public GameObject objetoAguaNaranja;

    private ObjectSelected miObjectSelected;

    void Start()
    {
     
        miObjectSelected = GetComponent<ObjectSelected>();

        if (miObjectSelected != null)
        {
            miObjectSelected.puedoInteractuar = false;
        }

        if (objetoAguaAzul != null) objetoAguaAzul.SetActive(true);
        if (objetoAguaNaranja != null) objetoAguaNaranja.SetActive(false);
        if (textoContador != null) textoContador.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        IngredienteCocinable ingrediente = other.GetComponent<IngredienteCocinable>();

        if (ingrediente != null)
        {
            if (ingrediente.tipo == IngredienteCocinable.TipoIngrediente.Daikon && !tieneDaikon)
            {
                tieneDaikon = true;
                Debug.Log("Daikon adentro");
                ingrediente.EntrarAOlla();
            }
            else if (ingrediente.tipo == IngredienteCocinable.TipoIngrediente.Cebollin && !tieneCebollin)
            {
                tieneCebollin = true;
                Debug.Log("Cebollín adentro");
                ingrediente.EntrarAOlla();
            }
            else if (ingrediente.tipo == IngredienteCocinable.TipoIngrediente.Mijo && !tieneMijo)
            {
                tieneMijo = true;
                Debug.Log("Mijo adentro");
                ingrediente.EntrarAOlla();
            }

            VerificarInicioCoccion();
        }
    }

    void VerificarInicioCoccion()
    {
        if (tieneDaikon && tieneCebollin && tieneMijo && !estaCocinando)
        {
            StartCoroutine(RutinaCoccion());
        }
    }

    IEnumerator RutinaCoccion()
    {
        estaCocinando = true;

        if (miObjectSelected != null)
        {
            miObjectSelected.puedoInteractuar = false;
        }

        if (textoContador != null) textoContador.gameObject.SetActive(true);

        float tiempoRestante = tiempoCoccion;

        while (tiempoRestante > 0f)
        {
            tiempoRestante -= Time.deltaTime;

            if (textoContador != null)
            {
                textoContador.text = Mathf.Ceil(tiempoRestante).ToString();
            }

            yield return null;
        }


        if (textoContador != null)
        {
            textoContador.text = "Listo";
        }

  
        if (objetoAguaAzul != null) objetoAguaAzul.SetActive(false);
        if (objetoAguaNaranja != null) objetoAguaNaranja.SetActive(true);


        if (miObjectSelected != null)
        {
            miObjectSelected.puedoInteractuar = true;
        }

        Debug.Log("Sopa");
    }
}
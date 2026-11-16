using UnityEngine;
using System.Collections.Generic;

public class ObjetoEmpacado : MonoBehaviour
{
    public string nombreObjeto = "Utensilio";
    public bool esBowl = false;

    public GameObject[] elementosActivarAlTerminar;


    public List<MonoBehaviour> scriptsAControlar = new List<MonoBehaviour>();


    public float alturaApoyoY = 0f;
    public float alturaExtraApilado = 0.08f;


    public bool usarRotacionPersonalizada = false;
    public Vector3 rotacionInicialEuler = Vector3.zero;

    private Collider[] todosLosColliders;
    private Rigidbody[] todosLosRigidbodies;
    private Renderer[] todosLosRenderers;
    private Dictionary<Renderer, Material[]> materialesOriginales = new Dictionary<Renderer, Material[]>();
    private bool mostrandoBlanco = false;

    private Quaternion rotacionBase;

    private Dictionary<Transform, Vector3> posicionesLocalesHijos = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> rotacionesLocalesHijos = new Dictionary<Transform, Quaternion>();

    void Awake()
    {
        todosLosColliders = GetComponentsInChildren<Collider>(true);
        todosLosRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        todosLosRenderers = GetComponentsInChildren<Renderer>(true);

        foreach (Transform hijo in GetComponentsInChildren<Transform>(true))
        {
            if (hijo != transform)
            {
                posicionesLocalesHijos[hijo] = hijo.localPosition;
                rotacionesLocalesHijos[hijo] = hijo.localRotation;
            }
        }

        if (usarRotacionPersonalizada)
        {
            rotacionBase = Quaternion.Euler(rotacionInicialEuler);
        }
        else
        {
            rotacionBase = transform.rotation;
        }

        foreach (Renderer rend in todosLosRenderers)
        {
            materialesOriginales[rend] = rend.sharedMaterials;
        }

        AsegurarScriptsEstado(false);
    }

    void RestaurarPosicionesHijos()
    {
        foreach (var kvp in posicionesLocalesHijos)
        {
            if (kvp.Key != null)
            {
                kvp.Key.localPosition = kvp.Value;
                kvp.Key.localRotation = rotacionesLocalesHijos[kvp.Key];
            }
        }
    }

    public void GuardarEnCesta()
    {
        foreach (var elem in elementosActivarAlTerminar)
        {
            if (elem != null) elem.SetActive(false);
        }
        AsegurarScriptsEstado(false);
        gameObject.SetActive(false);
    }

    public void PrepararParaColocar()
    {
        gameObject.SetActive(true);
        AsegurarScriptsEstado(false);

        foreach (var rb in todosLosRigidbodies)
        {
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }
        foreach (var col in todosLosColliders)
        {
            col.enabled = false;
        }

        RestaurarPosicionesHijos();
    }

    public void ActualizarVisualValidez(bool esValido, Material materialInvalidoBlanco)
    {
        if (esValido && mostrandoBlanco)
        {
            foreach (Renderer rend in todosLosRenderers)
            {
                if (rend != null && materialesOriginales.ContainsKey(rend))
                {
                    rend.sharedMaterials = materialesOriginales[rend];
                }
            }
            mostrandoBlanco = false;
        }
        else if (!esValido && !mostrandoBlanco && materialInvalidoBlanco != null)
        {
            foreach (Renderer rend in todosLosRenderers)
            {
                if (rend != null)
                {
                    Material[] matsBlancos = new Material[rend.sharedMaterials.Length];
                    for (int i = 0; i < matsBlancos.Length; i++)
                    {
                        matsBlancos[i] = materialInvalidoBlanco;
                    }
                    rend.sharedMaterials = matsBlancos;
                }
            }
            mostrandoBlanco = true;
        }
    }

    public void ActualizarTransformConRotacion(Vector3 nuevaPosicion, float giroYExtra)
    {
        transform.position = nuevaPosicion + Vector3.up * alturaApoyoY;

        Quaternion rotacionGiroY = Quaternion.Euler(0f, giroYExtra, 0f);
        transform.rotation = rotacionGiroY * rotacionBase;

        RestaurarPosicionesHijos();
    }

    public void ConfirmarColocacion(Vector3 posicionGrid, float giroYExtra)
    {
        ActualizarVisualValidez(true, null);
        ActualizarTransformConRotacion(posicionGrid, giroYExtra);

        foreach (var col in todosLosColliders)
        {
            col.enabled = true;
        }

        AsegurarScriptsEstado(false);
    }

    public void ActivarParaCocina()
    {
        foreach (var elem in elementosActivarAlTerminar)
        {
            if (elem != null) elem.SetActive(true);
        }

        AsegurarScriptsEstado(true);

        foreach (var rb in todosLosRigidbodies)
        {
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }

        RestaurarPosicionesHijos();
    }

    void AsegurarScriptsEstado(bool encender)
    {
        foreach (var script in scriptsAControlar)
        {
            if (script != null)
            {
                script.enabled = encender;
            }
        }
    }
}
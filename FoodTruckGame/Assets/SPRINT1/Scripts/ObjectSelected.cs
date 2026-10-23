using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using DentedPixel;

public class ObjectSelected : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

    Outline outline;
    public string message;
    public string messageSoltar;

    public TMPro.TextMeshProUGUI textoMensaje;
    public TMPro.TextMeshProUGUI textoSoltar;

    public UnityEvent onInteraction;

    public bool puedoInteractuar = true;

    public float distanciaMaxima = 6f;
    private Transform jugadorTransform;

    public GameObject objetoBrillo;


    //tooltip texto
    public string Contenido;
    public string Titulo;

    //delay
    private LTDescr delay;

    public static bool hayObjetoAgarrado = false;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        outline = GetComponent<Outline>();
        DisableOutline();

        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            jugadorTransform = jugador.transform;
        }
    }

    bool EstaEnRango()
    {
        if (jugadorTransform == null) return true;

        float distancia = Vector3.Distance(transform.position, jugadorTransform.position);
        return distancia <= distanciaMaxima;
    }

    public void Interact()
    {
        if (!puedoInteractuar || !EstaEnRango() || hayObjetoAgarrado) return;
        onInteraction.Invoke();
    }

    public void DisableOutline()
    {
        if (outline != null) outline.enabled = false;
    }

    public void EnableOutline()
    {
        if (outline != null) outline.enabled = true;
    }

    // HOVER OUTLINE
    private void OnMouseEnter()
    {
        if (!puedoInteractuar || !EstaEnRango() || hayObjetoAgarrado) return;

        if (objectGrabPointTransform == null && outline != null)
        {
            delay = LeanTween.delayedCall(0.5f, () =>
            {

                TooltipSystem.Show(Contenido, Titulo);
            });

            EnableOutline();
            if (textoMensaje != null)
            {
                textoMensaje.text = message;
            }
        }
    }

    private void OnMouseExit()
    {
        if (delay != null)
        {
            LeanTween.cancel(delay.uniqueId);
            delay = null;
        }

        TooltipSystem.Hide();

        if (outline != null)
        {
            DisableOutline();
            if (textoMensaje != null)
            {
                textoMensaje.text = "";
            }
        }
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        if (!EstaEnRango()) return;

        if (delay != null)
        {
            LeanTween.cancel(delay.uniqueId);
            delay = null;
        }
        TooltipSystem.Hide();

        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.isKinematic = true;
        objectRigidbody.useGravity = false;
        DisableOutline();

        hayObjetoAgarrado = true;

        if (objetoBrillo != null)
        {
            Destroy(objetoBrillo);
        }
    }

    public void Drop()
    {
        if (!puedoInteractuar) return;

        this.objectGrabPointTransform = null;
        objectRigidbody.isKinematic = false;
        objectRigidbody.useGravity = true;

        hayObjetoAgarrado = false;
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float lerpSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectRigidbody.MovePosition(newPosition);
        }
    }
}
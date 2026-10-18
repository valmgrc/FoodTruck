using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

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
        else
        {
           
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
        if (!puedoInteractuar) return;
        onInteraction.Invoke();
    }
    public void DisableOutline()
    {
        outline.enabled = false;
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    //HOVER OUTLINE
    private void OnMouseEnter()
    {
        if (!puedoInteractuar || !EstaEnRango()) return;
        if (objectGrabPointTransform == null && outline != null)
        {
            EnableOutline();
            if (textoMensaje != null)
            {
                textoMensaje.text = message;
            }
        }
    }

    private void OnMouseExit()
    {
        if (outline != null)
        {
            DisableOutline();
            if (textoMensaje != null)
            {
                textoMensaje.text = "";
            }
        }
    }

    //INTERACCION AGARRAR SOLTAR
    public void Grab(Transform objectGrabPointTransform)
    {
        if (!EstaEnRango()) return;

        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.isKinematic = true;
        objectRigidbody.useGravity = false;
        DisableOutline();
    }

    public void Drop()
    {
        if (!puedoInteractuar) return;

        this.objectGrabPointTransform = null;
        objectRigidbody.isKinematic = false;
        objectRigidbody.useGravity = true;
      
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

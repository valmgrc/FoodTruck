using UnityEngine;
using UnityEngine.Events;
using DentedPixel;

public class ObjectSelected : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

    Outline outline;

    public string mensajeAccion = "AGARRAR";      
    public string teclaAccion = "E";        

    public UnityEvent onInteraction;
    public bool puedoInteractuar = true;
    public float distanciaMaxima = 6f;
    private Transform jugadorTransform;

    public GameObject objetoBrillo;


    public string Contenido;
    public string Titulo;
    private LTDescr delay;

    public static bool hayObjetoAgarrado = false;
    public float velocidadRotacion = 200f;

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

    void Update()
    {
        if (objectGrabPointTransform != null)
        {
            if (!gameObject.CompareTag("Cuchillo") && !gameObject.CompareTag("Cucharon"))
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll != 0f)
                {
                    Transform camTransform = Camera.main != null ? Camera.main.transform : null;

                    if (camTransform != null)
                    {
                        transform.Rotate(camTransform.right, scroll * velocidadRotacion, Space.World);
                    }
                }
            }
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

    // HOVER OUTLINE Y MENSAJE


    private void OnMouseEnter()
    {
        if (objectGrabPointTransform != null) return;

        if (!puedoInteractuar || !EstaEnRango()) return;

        if (delay != null) LeanTween.cancel(delay.uniqueId);
        delay = LeanTween.delayedCall(0.1f, () => {

            ControladorOlla olla = GetComponent<ControladorOlla>();
            if (olla != null)
            {
                string infoOlla = "";

                if (olla.cantidadDaikon > 0) infoOlla += $"• Daikon                x{olla.cantidadDaikon}\n";
                if (olla.cantidadCebollin > 0) infoOlla += $"• Cebollín             x{olla.cantidadCebollin}\n";
                if (olla.cantidadMijo > 0) infoOlla += $"• Mijo                    x{olla.cantidadMijo}\n";

                if (olla.porcionesCocinandose > 0) infoOlla += $"\nCocinando: {olla.porcionesCocinandose} porción";
                if (olla.porcionesDisponiblesEnOlla > 0) infoOlla += $"\nListas: {olla.porcionesDisponiblesEnOlla} porción";

                if (string.IsNullOrEmpty(infoOlla)) infoOlla = "Olla vacía (Ninguna porción)";

                TooltipSystem.Show(infoOlla, Titulo);
            }
            else
            {
                TooltipSystem.Show(Contenido, Titulo);
            }
        });

        if (outline != null)
        {
            EnableOutline();

            if (ButtonPromptManager.Instance != null)
            {
                if (hayObjetoAgarrado)
                {
                    if (CucharonSystem.cucharonEnMano && (gameObject.CompareTag("Vasija") || name.Contains("Vasija")))
                    {
                        CucharonSystem cucharon = FindObjectOfType<CucharonSystem>();
                        if (cucharon != null && cucharon.tieneSopa)
                        {
                            ButtonPromptManager.Instance.MostrarDoblePrompt("E", "SOLTAR", "ClickIzq", "SERVIR");
                            return;
                        }
                    }

                    if (gameObject.CompareTag("Olla") || name.Contains("Olla"))
                    {
                        ButtonPromptManager.Instance.MostrarDoblePrompt("E", "SOLTAR", "ClickIzq", "COLOCAR");
                        return;
                    }

                    if (SistemaCorteObjeto.cuchilloEnMano)
                    {
                        SistemaCorteObjeto sistemaCorte = GetComponent<SistemaCorteObjeto>();
                        if (sistemaCorte != null && sistemaCorte.clicsActuales < sistemaCorte.clicsNecesarios)
                        {
                            ButtonPromptManager.Instance.MostrarDoblePrompt("E", "SOLTAR", "ClickIzq", "CORTAR");
                            return;
                        }
                    }

                    if (CucharonSystem.cucharonEnMano && (gameObject.CompareTag("Olla") || name.Contains("Olla")))
                    {
                        ControladorOlla olla = GetComponent<ControladorOlla>();
                        if (olla != null && olla.objetoAguaNaranja != null && olla.objetoAguaNaranja.activeSelf)
                        {
                            ButtonPromptManager.Instance.MostrarDoblePrompt("E", "SOLTAR", "ClickIzq", "CUCHAREAR");
                            return;
                        }
                    }

                    ButtonPromptManager.Instance.MostrarPrompt("E", "SOLTAR");
                }
                else
                {
                    ButtonPromptManager.Instance.MostrarPrompt(teclaAccion, mensajeAccion);
                }
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

            if (ButtonPromptManager.Instance != null)
            {
                if (ObjectSelected.hayObjetoAgarrado)
                {
                    bool esRotable = !gameObject.CompareTag("Cuchillo") && !gameObject.CompareTag("Cucharon")
                                     && !name.Contains("Cuchillo") && !name.Contains("Cucharon");

                    if (esRotable)
                    {
                        ButtonPromptManager.Instance.MostrarDoblePrompt("E", "SOLTAR", "Rueda", "ROTAR");
                    }
                    else
                    {
                        ButtonPromptManager.Instance.MostrarPrompt("E", "SOLTAR");
                    }
                }
                else
                {
                    ButtonPromptManager.Instance.OcultarPrompts();
                }
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

        bool esRotable = !gameObject.CompareTag("Cuchillo") && !gameObject.CompareTag("Cucharon")
                         && !name.Contains("Cuchillo") && !name.Contains("Cucharon");

        if (ButtonPromptManager.Instance != null)
        {
            if (esRotable)
            {
                ButtonPromptManager.Instance.MostrarDoblePrompt("E", "SOLTAR", "Rueda", "ROTAR");
            }
            else
            {
                ButtonPromptManager.Instance.MostrarPrompt("E", "SOLTAR");
            }
        }
    }

    public void Drop()
    {
        if (!puedoInteractuar) return;

        this.objectGrabPointTransform = null;
        objectRigidbody.isKinematic = false;
        objectRigidbody.useGravity = true;

        hayObjetoAgarrado = false;

        if (ButtonPromptManager.Instance != null)
        {
            ButtonPromptManager.Instance.OcultarPrompts();
        }
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
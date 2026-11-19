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

    public static ObjectSelected objetoAgarradoActual = null;
    public bool bloquearRotacion = false;

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
            if (!bloquearRotacion)
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

        if (ObjectSelected.hayObjetoAgarrado && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 6f))
            {
                VasijaTelaController vasija = hit.collider.GetComponent<VasijaTelaController>();
                if (vasija != null)
                {
                    vasija.ColocarTela();
                }

                Especia especia = hit.collider.GetComponent<Especia>();
                if (especia != null && ObjectSelected.objetoAgarradoActual != null)
                {
                    especia.CargarEnCuchara();
                }
            }

            PlatoLargoController platoLargo = hit.collider.GetComponent<PlatoLargoController>();
            if (platoLargo != null && ObjectSelected.objetoAgarradoActual != null)
            {
                GameObject objMano = ObjectSelected.objetoAgarradoActual.gameObject;

                if (!platoLargo.tienePescado && (objMano.CompareTag("PescadoCortado") || objMano.name.Contains("PescadoCortado")))
                {
                    platoLargo.ColocarPescado(ObjectSelected.objetoAgarradoActual);
                }
                else if (platoLargo.tienePescado && !platoLargo.tieneVino && !platoLargo.estaVertiendo &&
                        (objMano.CompareTag("VinoArroz") || objMano.name.Contains("VinoArroz")))
                {
                    platoLargo.IniciarVertidoVino(ObjectSelected.objetoAgarradoActual);
                }
                else if (platoLargo.tienePescado && platoLargo.tieneVino && !platoLargo.tieneSal &&
                        (objMano.CompareTag("CucharaEspecias") || objMano.name.Contains("Cuchara")))
                {
                    CucharitaEspecias cucharaSys = objMano.GetComponent<CucharitaEspecias>();
                    if (cucharaSys != null && cucharaSys.tieneSalCargada)
                    {
                        platoLargo.AplicarSal(ObjectSelected.objetoAgarradoActual);
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

    bool EsVerduraCortadaEnMano()
    {
        if (!hayObjetoAgarrado || objetoAgarradoActual == null) return false;

        GameObject objMano = objetoAgarradoActual.gameObject;
        IngredienteCocinable ingrediente = objMano.GetComponent<IngredienteCocinable>();

        if (ingrediente != null)
        {
            return true;
        }

        if (objMano.CompareTag("Daikon") || objMano.CompareTag("Cebollin") || objMano.CompareTag("Mijo") ||
            objMano.name.Contains("Cortado") || objMano.name.Contains("Daikon") ||
            objMano.name.Contains("Cebollin") || objMano.name.Contains("Mijo"))
        {
            return true;
        }

        return false;
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
                        if (CucharonSystem.cucharonEnMano)
                        {
                            ButtonPromptManager.Instance.MostrarDoblePrompt("E", "SOLTAR", "ClickIzq", "SERVIR");
                            return;
                        }

                        if (EsVerduraCortadaEnMano())
                        {
                            ButtonPromptManager.Instance.MostrarPrompt("E", "COLOCAR EN OLLA");
                            return;
                        }

                        ButtonPromptManager.Instance.MostrarPrompt("E", "SOLTAR");
                        return;
                    }

                    if (gameObject.CompareTag("Vasija") || name.Contains("Vasija"))
                    {
                        VasijaTelaController vasijaTela = GetComponent<VasijaTelaController>();
                        if (vasijaTela != null && !vasijaTela.estaTapada && ObjectSelected.objetoAgarradoActual != null)
                        {
                            GameObject objMano = ObjectSelected.objetoAgarradoActual.gameObject;
                            if (objMano.CompareTag("Tela") || objMano.name.Contains("Tela"))
                            {
                                ButtonPromptManager.Instance.MostrarDoblePrompt("E", "SOLTAR", "ClickIzq", "COLOCAR");
                                return;
                            }
                        }
                    }

                    if (gameObject.CompareTag("PlatoLargo") || name.Contains("PlatoLargo"))
                    {
                        PlatoLargoController platoLargo = GetComponent<PlatoLargoController>();
                        if (platoLargo != null && ObjectSelected.objetoAgarradoActual != null)
                        {
                            GameObject objMano = ObjectSelected.objetoAgarradoActual.gameObject;

                            if (!platoLargo.tienePescado && (objMano.CompareTag("PescadoCortado") || objMano.name.Contains("PescadoCortado")))
                            {
                                ButtonPromptManager.Instance.MostrarDoblePrompt("E", "SOLTAR", "ClickIzq", "COLOCAR");
                                return;
                            }
                            else if (platoLargo.tienePescado && !platoLargo.tieneVino && !platoLargo.estaVertiendo &&
                                    (objMano.CompareTag("VinoArroz") || objMano.name.Contains("VinoArroz")))
                            {
                                ButtonPromptManager.Instance.MostrarDoblePrompt("E", "SOLTAR", "ClickIzq", "VERTIR");
                                return;
                            }
                            else if (platoLargo.tienePescado && platoLargo.tieneVino && !platoLargo.tieneSal &&
                                    (objMano.CompareTag("CucharaEspecias") || objMano.name.Contains("Cuchara")))
                            {
                                CucharitaEspecias cucharaSys = objMano.GetComponent<CucharitaEspecias>();
                                if (cucharaSys != null && cucharaSys.tieneSalCargada)
                                {
                                    ButtonPromptManager.Instance.MostrarDoblePrompt("E", "SOLTAR", "ClickIzq", "ECHAR SAL");
                                    return;
                                }
                            }
                        }
                    }

                    if (gameObject.CompareTag("Cesta") || name.Contains("Cesta"))
                    {
                        ButtonPromptManager.Instance.MostrarPrompt("ClickIzq", "SACAR OBJETO");
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
                    bool esRotable = !gameObject.CompareTag("Cuchillo") && !gameObject.CompareTag("Cucharon") && !gameObject.CompareTag("CucharaEspecias")
                                     && !name.Contains("Cuchillo") && !name.Contains("Cucharon") && !name.Contains("Cucharita");

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
        objectRigidbody.detectCollisions = false;

        DisableOutline();

        hayObjetoAgarrado = true;
        objetoAgarradoActual = this;

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
        objectRigidbody.detectCollisions = true;

        hayObjetoAgarrado = false;
        if (objetoAgarradoActual == this) objetoAgarradoActual = null;

        if (ButtonPromptManager.Instance != null)
        {
            ButtonPromptManager.Instance.OcultarPrompts();
        }
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float lerpSpeed = 15f;

            transform.position = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);

            if (gameObject.CompareTag("Cucharon") || name.Contains("Cucharon") ||
                gameObject.CompareTag("CucharaEspecias") || name.Contains("Cucharita"))
            {
                Quaternion targetRotation = objectGrabPointTransform.rotation * Quaternion.Euler(0f, 0f, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lerpSpeed);
            }
        }
    }
}
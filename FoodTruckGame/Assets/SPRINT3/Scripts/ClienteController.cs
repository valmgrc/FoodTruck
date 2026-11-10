using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ClienteController : MonoBehaviour
{
    private Vector3 destinoActual;
    private bool esperandoEnPunto = false;
    public float velocidadCaminata = 2.5f;

    public GameObject panelPedidoUI;
    public Image imagenPlato;
    public Image imagenModalidad;
    public TextMeshProUGUI textoContemporizador;
    public Vector3 offsetPantalla = new Vector3(0, 2f, 0);
    private Camera camaraJugador;



    [HideInInspector] public int idPuntoAsignado;
    [HideInInspector] public GeneradorClientes generadorRef;

    public Sprite[] spritesPlatos;
    public Sprite[] spritesModalidades;

    private int platoSeleccionadoIndex = 0;
    private int modalidadSeleccionadaIndex = 0;


    public float tiempoMaximoEspera = 30f;
    private float tiempoRestante;
    private Coroutine corutinaTemporizador;


    private Outline outline;

    private void Awake()
    {
        camaraJugador = Camera.main;

        outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
    }

    public void InicializarCliente(Vector3 puntoDestino)
    {
        destinoActual = puntoDestino;

        if (panelPedidoUI != null) panelPedidoUI.SetActive(false);

        ElegirPedidoAleatorio();
        StartCoroutine(RutinaMoverAlDestino());
    }

    void Update()
    {
        if (esperandoEnPunto && panelPedidoUI != null && panelPedidoUI.activeSelf && camaraJugador != null)
        {
            Vector3 posicionPantalla = camaraJugador.WorldToScreenPoint(transform.position + offsetPantalla);
            if (posicionPantalla.z > 0)
            {
                panelPedidoUI.transform.position = posicionPantalla;
            }
        }

        if (esperandoEnPunto && camaraJugador != null)
        {
            Ray ray = camaraJugador.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); 
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 5f))
            {
                if (hit.transform == transform || hit.transform.IsChildOf(transform))
                {

                    if (outline != null) outline.enabled = true;

                    if (ObjectSelected.hayObjetoAgarrado && ObjectSelected.objetoAgarradoActual != null)
                    {
                        ControladorBowl bowl = ObjectSelected.objetoAgarradoActual.GetComponent<ControladorBowl>();
                        ControladorCajaLlevar caja = ObjectSelected.objetoAgarradoActual.GetComponent<ControladorCajaLlevar>();

                        bool tieneBowlLleno = (bowl != null && bowl.estaLleno);
                        bool tieneCajaLlena = (caja != null && caja.estaLleno);

                        if (tieneBowlLleno || tieneCajaLlena)
                        {
                            if (ButtonPromptManager.Instance != null)
                            {
                                ButtonPromptManager.Instance.MostrarPrompt("ClickIzq", "ENTREGAR");
                            }

                            if (Input.GetMouseButtonDown(0)) //entregar comidita
                            {
                                ProcesarEntrega(bowl, caja);
                            }
                        }
                    }
                    return;
                }
            }

            if (outline != null) outline.enabled = false;
        }
    }

    void ProcesarEntrega(ControladorBowl bowl, ControladorCajaLlevar caja)
    {
        bool esBowlValido = (bowl != null && bowl.estaLleno);
        bool esCajaValida = (caja != null && caja.estaLleno);

        bool entregaCorrecta = false;

        if (modalidadSeleccionadaIndex == 0 && esBowlValido) entregaCorrecta = true;
        else if (modalidadSeleccionadaIndex == 1 && esCajaValida) entregaCorrecta = true;

        if (entregaCorrecta)
        {
            ObjectSelected.hayObjetoAgarrado = false;
            ObjectSelected.objetoAgarradoActual = null;

            Destroy(bowl != null ? bowl.gameObject : caja.gameObject);

            FinalizarAtencion();
        }
        else
        {
            tiempoRestante -= 5f;

            if (textoContemporizador != null)
            {
                textoContemporizador.text = Mathf.Max(0, Mathf.CeilToInt(tiempoRestante)).ToString() + "s";
            }

            if (tiempoRestante <= 0f)
            {
                FinalizarAtencion();
            }
        }
    }

    void ElegirPedidoAleatorio()
    {
        if (spritesPlatos != null && spritesPlatos.Length > 0)
        {
            platoSeleccionadoIndex = Random.Range(0, spritesPlatos.Length);
        }
        if (spritesModalidades != null && spritesModalidades.Length > 0)
        {
            modalidadSeleccionadaIndex = Random.Range(0, spritesModalidades.Length);
        }
    }

    IEnumerator RutinaMoverAlDestino()
    {
        esperandoEnPunto = false;

        Vector3 direccion = (destinoActual - transform.position).normalized;
        direccion.y = 0;
        if (direccion != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direccion);
        }

        while (Vector3.Distance(transform.position, destinoActual) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destinoActual, velocidadCaminata * Time.deltaTime);
            yield return null;
        }

        transform.position = destinoActual;
        esperandoEnPunto = true;
        MostrarPedido();

        corutinaTemporizador = StartCoroutine(RutinaContadorPaciencia());
    }

    void MostrarPedido()
    {
        if (panelPedidoUI != null)
        {
            panelPedidoUI.SetActive(true);
            tiempoRestante = tiempoMaximoEspera;

            if (imagenPlato != null && spritesPlatos != null && spritesPlatos.Length > 0)
            {
                imagenPlato.sprite = spritesPlatos[platoSeleccionadoIndex];
            }

            if (imagenModalidad != null && spritesModalidades != null && spritesModalidades.Length > 0)
            {
                imagenModalidad.sprite = spritesModalidades[modalidadSeleccionadaIndex];
            }
        }
    }

    IEnumerator RutinaContadorPaciencia()
    {
        while (esperandoEnPunto && tiempoRestante > 0f)
        {
            tiempoRestante -= Time.deltaTime;

            if (textoContemporizador != null)
            {
                textoContemporizador.text = Mathf.Max(0, Mathf.CeilToInt(tiempoRestante)).ToString() + "s";
            }

            yield return null;
        }

        if (esperandoEnPunto)
        {
            FinalizarAtencion();
        }
    }

    private void OnMouseEnter()
    {
        if (!esperandoEnPunto) return;

        if (outline != null) outline.enabled = true;

        if (ButtonPromptManager.Instance != null && ObjectSelected.hayObjetoAgarrado && ObjectSelected.objetoAgarradoActual != null)
        {
            ControladorBowl bowl = ObjectSelected.objetoAgarradoActual.GetComponent<ControladorBowl>();
            ControladorCajaLlevar caja = ObjectSelected.objetoAgarradoActual.GetComponent<ControladorCajaLlevar>();

            bool tieneBowlLleno = (bowl != null && bowl.estaLleno);
            bool tieneCajaLlena = (caja != null && caja.estaLleno);

            if (tieneBowlLleno || tieneCajaLlena)
            {
                ButtonPromptManager.Instance.MostrarPrompt("ClickIzq", "ENTREGAR");
            }
        }
    }

    private void OnMouseExit()
    {
        if (outline != null) outline.enabled = false;

        if (ButtonPromptManager.Instance != null)
        {
            ButtonPromptManager.Instance.OcultarPrompts();
        }
    }

    private void OnMouseDown()
    {
        if (!esperandoEnPunto || !ObjectSelected.hayObjetoAgarrado || ObjectSelected.objetoAgarradoActual == null) return;

        ControladorBowl bowl = ObjectSelected.objetoAgarradoActual.GetComponent<ControladorBowl>();
        ControladorCajaLlevar caja = ObjectSelected.objetoAgarradoActual.GetComponent<ControladorCajaLlevar>();

        bool esBowlValido = (bowl != null && bowl.estaLleno);
        bool esCajaValida = (caja != null && caja.estaLleno);

        if (!esBowlValido && !esCajaValida) return;

        bool entregaCorrecta = false;

        if (modalidadSeleccionadaIndex == 0 && esBowlValido)
        {
            entregaCorrecta = true;
        }
        else if (modalidadSeleccionadaIndex == 1 && esCajaValida)
        {
            entregaCorrecta = true;
        }

        if (entregaCorrecta)
        {
            ObjectSelected.hayObjetoAgarrado = false;
            ObjectSelected.objetoAgarradoActual = null;

            Destroy(bowl != null ? bowl.gameObject : caja.gameObject);

            FinalizarAtencion();
        }
        else
        {
            tiempoRestante -= 5f;

            if (textoContemporizador != null)
            {
                textoContemporizador.text = Mathf.Max(0, Mathf.CeilToInt(tiempoRestante)).ToString() + "s";
            }

            if (tiempoRestante <= 0f)
            {
                FinalizarAtencion();
            }
        }
    }

    void FinalizarAtencion()
    {
        esperandoEnPunto = false;

        if (corutinaTemporizador != null)
        {
            StopCoroutine(corutinaTemporizador);
        }

        if (panelPedidoUI != null) panelPedidoUI.SetActive(false);
        if (outline != null) outline.enabled = false;

        if (ButtonPromptManager.Instance != null)
        {
            ButtonPromptManager.Instance.OcultarPrompts();
        }
        if (generadorRef != null)
        {
            generadorRef.LiberarPunto(idPuntoAsignado);
        }

        StartCoroutine(RutinaRetiroCliente());
    }

    IEnumerator RutinaRetiroCliente()
    {
        float tiempoRotacion = 0.6f;
        float transcurridos = 0f;
        Quaternion rotacionInicial = transform.rotation;
        Quaternion rotacionFinal = transform.rotation * Quaternion.Euler(0, 180f, 0);

        while (transcurridos < tiempoRotacion)
        {
            transcurridos += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(rotacionInicial, rotacionFinal, transcurridos / tiempoRotacion);
            yield return null;
        }

        float tiempoCaminata = 4f;
        transcurridos = 0f;
        while (transcurridos < tiempoCaminata)
        {
            transcurridos += Time.deltaTime;
            transform.Translate(Vector3.forward * (velocidadCaminata * Time.deltaTime));
            yield return null;
        }

        Destroy(gameObject);
    }
}
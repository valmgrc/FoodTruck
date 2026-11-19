using DentedPixel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    public GameObject panelQuest1;
    public TextMeshProUGUI textoTitulo1;
    public TextMeshProUGUI textoContenido1;
    private CanvasGroup canvasGroup1;

    public GameObject panelQuest2;
    public TextMeshProUGUI textoTitulo2;
    public TextMeshProUGUI textoContenido2;
    private CanvasGroup canvasGroup2;


    public GameObject panelRecetarioVisual;  
    public GameObject panelEmpaquetadoVisual;
    public GameObject iconoTooltipTeclaN; 
    private bool recetarioAbierto = false;
    private bool enFasePedidos = false;

    public float duracionFade = 0.3f;

    public GameObject panelFadeNegro;
    public CanvasGroup canvasGroupFade;
    public Transform puntoInicioPreparacion;
    public TextMeshProUGUI textoPreparate;


    public GameObject panelPopUpCocina;      
    public GameObject tooltipTeclaP;    
    public MonoBehaviour scriptMovimientoJugador; 
    private bool popUpCocinaAbierto = false;
    private bool esperandoAbrirFoodTruck = false;

    public GeneradorClientes generadorClientes;
    public Transform jugadorTransform;
    public Transform puntoInicioClientes;


    public List<GameObject> objetosADesactivar = new List<GameObject>();
    public List<GameObject> objetosGuiaOrganizacion = new List<GameObject>(); //pa que no se vea su outline de los fijos

    public int totalVentanas = 2;
    private int ventanasAbiertas = 0;
    [HideInInspector] public bool esperandoAperturaVentanas = false;

    private bool misionMovimientoActiva = true;
    private bool misionRecetarioActiva = false;
    private bool misionEmpaquetadoActiva = false;

    private bool misionCortarActiva = false;
    private bool misionHervirActiva = false;
    private bool misionServirActiva = false;
    private bool misionTipDuplicarActiva = false;

    private bool transicionIniciada = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        canvasGroup1 = panelQuest1.GetComponent<CanvasGroup>();
        if (canvasGroup1 == null) canvasGroup1 = panelQuest1.AddComponent<CanvasGroup>();

        canvasGroup2 = panelQuest2.GetComponent<CanvasGroup>();
        if (canvasGroup2 == null) canvasGroup2 = panelQuest2.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        BloquearObjetoOrganizacion();

        MostrarQuest(1, "Tip", "Puedes moverte alrededor con W, A, S, D");
        Invoke("ActivarMisionRecetario", 1.5f);

        if (panelFadeNegro != null)
        {
            panelFadeNegro.SetActive(false);
            if (canvasGroupFade != null) canvasGroupFade.alpha = 0f;
        }
        if (generadorClientes != null) generadorClientes.enabled = false;
        if (textoPreparate != null) textoPreparate.gameObject.SetActive(false);

        if (panelPopUpCocina != null) panelPopUpCocina.SetActive(false);
        if (tooltipTeclaP != null) tooltipTeclaP.SetActive(false);
        if (panelEmpaquetadoVisual != null) panelEmpaquetadoVisual.SetActive(false);
    }

    void Update()
    {
        if (misionMovimientoActiva)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                misionMovimientoActiva = false;
                OcultarQuest(1);
            }
        }

        if (popUpCocinaAbierto)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CerrarYDestruirPopUpCocina();
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            recetarioAbierto = !recetarioAbierto;


            GameObject panelActivoN = enFasePedidos ? panelEmpaquetadoVisual : panelRecetarioVisual;

            if (panelActivoN != null)
            {
                panelActivoN.SetActive(recetarioAbierto);
            }

            if (recetarioAbierto)
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (!enFasePedidos && misionRecetarioActiva)
                {
                    misionRecetarioActiva = false;
                    OcultarQuest(2);
                    Invoke("ActivarMisionOrganizacion", 0.5f);
                }
                else if (enFasePedidos && misionEmpaquetadoActiva)
                {
                    misionEmpaquetadoActiva = false;
                    OcultarQuest(2);
                }
            }
        }

        if (esperandoAbrirFoodTruck && !recetarioAbierto)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                AlPresionarAbrirFoodTruck();
            }
        }

        if (esperandoAperturaVentanas && Input.GetMouseButtonDown(0) && !recetarioAbierto)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 6f))
            {
                VentanaPedidos ventana = hit.collider.GetComponentInParent<VentanaPedidos>();
                if (ventana != null && !ventana.estaAbierta)
                {
                    ventana.AbrirVentana();
                }
            }
        }
    }

    void ActivarMisionRecetario()
    {
        misionRecetarioActiva = true;
        MostrarQuest(2, "Tip", "Puedes revisar tu recetario con la tecla N");
    }

    void ActivarMisionOrganizacion()
    {
        MostrarQuest(1, "Organización", "Haz clic en las cestas para sacar tus utensilios y acomodarlos");
    }

    void IniciarMisionesCocina()
    {
        misionCortarActiva = true;
        MostrarQuest(1, "Preparación", "Dirígete al área de cortar para comenzar a cocinar");

        misionTipDuplicarActiva = true;
        MostrarQuest(2, "Tip", "Recuerda que puedes hacer 2 porciones de tu platillo al duplicar ingredientes");
    }

    public void MostrarQuestRevolverOlla()
    {
        MostrarQuest(2, "Cocina", "Asegúrate de revolver la olla con el cucharón manteniendo el clic.");
    }

    public void OcultarQuestRevolverOlla()
    {
        OcultarQuest(2);
    }

    public void NotificarIngredienteListoOSostenido()
    {
        if (misionCortarActiva)
        {
            misionCortarActiva = false;
            OcultarQuest(1);
            Invoke("ActivarMisionHervir", 1f);
        }
    }

    void ActivarMisionHervir()
    {
        misionHervirActiva = true;
        MostrarQuest(1, "Preparación", "Recuerda hervir tus ingredientes en la olla");
    }

    public void NotificarIngredientesEnOlla()
    {
        if (misionHervirActiva)
        {
            misionHervirActiva = false;
            OcultarQuest(1);
        }
    }
    public void NotificarSopaLista()
    {
        misionServirActiva = true;
        MostrarQuest(1, "Preparación", "Recuerda servir tu platillo en la vasija");
    }

    public void NotificarSopaServida()
    {
        if (misionServirActiva)
        {
            misionServirActiva = false;
            OcultarQuest(1);
        }
    }

    public void IniciarTransicionDesdeOrganizacion(List<ObjetoEmpacado> objetosAcomodados)
    {
        StartCoroutine(RutinaTransicionDesdeOrganizacion(objetosAcomodados));
    }

    IEnumerator RutinaTransicionDesdeOrganizacion(List<ObjetoEmpacado> objetosAcomodados)
    {
        OcultarQuest(1);
        OcultarQuest(2);

        MostrarQuest(1, "Preparación", "Tu carrito está acomodado. Prepárate para cocinar.");
        yield return new WaitForSeconds(4f);
        OcultarQuest(1);

        if (panelFadeNegro != null && canvasGroupFade != null)
        {
            panelFadeNegro.SetActive(true);
            canvasGroupFade.alpha = 0f;
            LeanTween.cancel(panelFadeNegro);
            LeanTween.value(panelFadeNegro, 0f, 1f, 1.5f)
                .setOnUpdate((float val) => { canvasGroupFade.alpha = val; });
        }

        yield return new WaitForSeconds(1.6f);

        if (jugadorTransform != null && puntoInicioPreparacion != null)
        {
            CharacterController controller = jugadorTransform.GetComponent<CharacterController>();
            MonoBehaviour scriptCamara = jugadorTransform.GetComponentInChildren<Camera>()?.GetComponent<MonoBehaviour>();

            if (controller != null) controller.enabled = false;
            if (scriptCamara != null) scriptCamara.enabled = false;

            jugadorTransform.position = puntoInicioPreparacion.position;
            jugadorTransform.rotation = puntoInicioPreparacion.rotation;

            Camera camaraMain = Camera.main;
            if (camaraMain != null && camaraMain.transform.IsChildOf(jugadorTransform))
            {
                camaraMain.transform.rotation = puntoInicioPreparacion.rotation;
                camaraMain.transform.localRotation = Quaternion.identity;
            }

            Physics.SyncTransforms();

            if (controller != null) controller.enabled = true;
            if (scriptCamara != null) scriptCamara.enabled = true;
        }

        yield return new WaitForSeconds(0.5f);

        if (panelFadeNegro != null && canvasGroupFade != null)
        {
            LeanTween.cancel(panelFadeNegro);
            LeanTween.value(panelFadeNegro, 1f, 0f, 1.5f)
                .setOnUpdate((float val) => { canvasGroupFade.alpha = val; })
                .setOnComplete(() => { panelFadeNegro.SetActive(false); });
        }

        yield return new WaitForSeconds(1.5f);

        if (objetosAcomodados != null)
        {
            foreach (var obj in objetosAcomodados)
            {
                if (obj != null) obj.ActivarParaCocina();
            }
        }
        HabilitarObjetoPreparacion();

        yield return new WaitForSeconds(1f);
        AbrirPopUpCocina();
    }


    void AbrirPopUpCocina()
    {
        if (panelPopUpCocina == null)
        {
            FinalizarCierrePopUp();
            return;
        }

        popUpCocinaAbierto = true;

        Time.timeScale = 0f;
        if (scriptMovimientoJugador != null)
        {
            scriptMovimientoJugador.enabled = false;
        }

        panelPopUpCocina.SetActive(true);
        LeanTween.cancel(panelPopUpCocina);
        panelPopUpCocina.transform.localScale = Vector3.zero;
        LeanTween.scale(panelPopUpCocina, Vector3.one, 0.45f)
            .setEaseOutBack()
            .setIgnoreTimeScale(true);
    }

    void CerrarYDestruirPopUpCocina()
    {
        if (!popUpCocinaAbierto) return;
        popUpCocinaAbierto = false;

        if (panelPopUpCocina != null)
        {
            LeanTween.cancel(panelPopUpCocina);
            Destroy(panelPopUpCocina);
            panelPopUpCocina = null;
        }

        FinalizarCierrePopUp();
    }

    void FinalizarCierrePopUp()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (scriptMovimientoJugador != null)
        {
            scriptMovimientoJugador.enabled = true;
        }

        if (tooltipTeclaP != null)
        {
            tooltipTeclaP.SetActive(true);
        }
        esperandoAbrirFoodTruck = true;

        IniciarMisionesCocina();
    }

    void BloquearObjetoOrganizacion()
    {
        foreach (GameObject obj in objetosGuiaOrganizacion)
        {
            if (obj == null) continue;

            ObjectSelected objSel = obj.GetComponent<ObjectSelected>();
            if (objSel != null)
            {
                objSel.puedoInteractuar = false;
                objSel.enabled = false;
            }

            Outline[] outlines = obj.GetComponentsInChildren<Outline>(true);
            foreach (Outline outl in outlines)
            {
                if (outl != null)
                {
                    outl.enabled = false;
                }
            }
        }
    }

    void HabilitarObjetoPreparacion()
    {
        foreach (GameObject obj in objetosGuiaOrganizacion)
        {
            if (obj == null) continue;

            ObjectSelected objSel = obj.GetComponent<ObjectSelected>();
            if (objSel != null)
            {
                objSel.enabled = true;
                objSel.puedoInteractuar = true;
            }
        }
    }

    public void AlPresionarAbrirFoodTruck()
    {
        if (transicionIniciada) return;

        esperandoAbrirFoodTruck = false;


        if (tooltipTeclaP != null)
        {
            tooltipTeclaP.SetActive(false);
        }
        if (iconoTooltipTeclaN != null)
        {
            iconoTooltipTeclaN.SetActive(false);
        }

        if (recetarioAbierto)
        {
            recetarioAbierto = false;
            if (panelRecetarioVisual != null) panelRecetarioVisual.SetActive(false);
            if (panelEmpaquetadoVisual != null) panelEmpaquetadoVisual.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        transicionIniciada = true;
        StartCoroutine(RutinaTransicionClientes());
    }


    IEnumerator RutinaTransicionClientes()
    {
        OcultarQuest(1);
        OcultarQuest(2);

        if (panelFadeNegro != null && canvasGroupFade != null)
        {
            panelFadeNegro.SetActive(true);
            canvasGroupFade.alpha = 0f;
            LeanTween.cancel(panelFadeNegro);
            LeanTween.value(panelFadeNegro, 0f, 1f, 1.5f)
                .setOnUpdate((float val) => { canvasGroupFade.alpha = val; });
        }

        yield return new WaitForSeconds(1.6f);

        if (jugadorTransform != null && puntoInicioClientes != null)
        {
            CharacterController controller = jugadorTransform.GetComponent<CharacterController>();
            MonoBehaviour scriptCamara = jugadorTransform.GetComponentInChildren<Camera>()?.GetComponent<MonoBehaviour>();

            if (controller != null) controller.enabled = false;
            if (scriptCamara != null) scriptCamara.enabled = false;

            jugadorTransform.position = puntoInicioClientes.position;
            jugadorTransform.rotation = puntoInicioClientes.rotation;

            Camera camaraMain = Camera.main;
            if (camaraMain != null && camaraMain.transform.IsChildOf(jugadorTransform))
            {
                camaraMain.transform.rotation = puntoInicioClientes.rotation;
                camaraMain.transform.localRotation = Quaternion.identity;
            }

            Physics.SyncTransforms();

            if (controller != null) controller.enabled = true;
            if (scriptCamara != null) scriptCamara.enabled = true;
        }

        if (textoPreparate != null)
        {
            textoPreparate.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        if (panelFadeNegro != null && canvasGroupFade != null)
        {
            LeanTween.cancel(panelFadeNegro);
            LeanTween.value(panelFadeNegro, 1f, 0f, 1.5f)
                .setOnUpdate((float val) => { canvasGroupFade.alpha = val; })
                .setOnComplete(() => { panelFadeNegro.SetActive(false); });
        }

        yield return new WaitForSeconds(1.5f);

        if (textoPreparate != null)
        {
            textoPreparate.gameObject.SetActive(false);
        }


        enFasePedidos = true;
        DesactivarScriptFasePedidos();

        if (iconoTooltipTeclaN != null)
        {
            iconoTooltipTeclaN.SetActive(true);
            LeanTween.cancel(iconoTooltipTeclaN);
            iconoTooltipTeclaN.transform.localScale = Vector3.zero;
            LeanTween.scale(iconoTooltipTeclaN, Vector3.one, 0.45f)
                .setEaseOutBack();
        }

        ventanasAbiertas = 0;
        esperandoAperturaVentanas = true;
        MostrarQuest(1, "Apertura", "Abre las ventanas para comenzar a recibir pedidos (0/2)");

        misionEmpaquetadoActiva = true;
        MostrarQuest(2, "Tip", "Presiona N para aprender cómo empaquetar los platillos");
    }
    void DesactivarScriptFasePedidos()
    {
        foreach (GameObject obj in objetosADesactivar)
        {
            if (obj == null) continue;

            ObjectSelected objSel = obj.GetComponent<ObjectSelected>();
            if (objSel != null)
            {
                objSel.puedoInteractuar = false;
                objSel.enabled = false;
            }

            Outline[] outlines = obj.GetComponentsInChildren<Outline>(true);
            foreach (Outline outl in outlines)
            {
                if (outl != null)
                {
                    outl.enabled = false;
                }
            }
        }
    }
    public void NotificarVentanaAbierta()
    {
        if (!esperandoAperturaVentanas) return;

        ventanasAbiertas++;

        if (ventanasAbiertas < totalVentanas)
        {
            MostrarQuest(1, "Apertura", $"Abre las ventanas para comenzar a recibir pedidos ({ventanasAbiertas}/{totalVentanas})");
        }
        else
        {
            esperandoAperturaVentanas = false;
            OcultarQuest(1);

            if (generadorClientes != null)
            {
                generadorClientes.enabled = true;
            }

            TiempoCerrar relojServicio = FindObjectOfType<TiempoCerrar>();
            if (relojServicio != null)
            {
                relojServicio.enabled = true;
                if (relojServicio.textoCronometro != null)
                {
                    relojServicio.textoCronometro.gameObject.SetActive(true);
                }
            }

            MostrarTipTemporal(1, "Tip", "Sírvele a los clientes el platillo correcto para recibir tu pago completo.", 15f);
            MostrarTipTemporal(2, "Entrega", "Sirve los platillos de la vasija usando el cucharón", 15f);
        }
    }

    public void MostrarTipTemporal(int numeroPanel, string titulo, string contenido, float tiempoVisibilidad)
    {
        MostrarQuest(numeroPanel, titulo, contenido);
        StartCoroutine(RutinaOcultarTip(numeroPanel, tiempoVisibilidad));
    }

    IEnumerator RutinaOcultarTip(int numeroPanel, float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        OcultarQuest(numeroPanel);
    }

    public void MostrarQuest(int numeroPanel, string titulo, string contenido)
    {
        GameObject panel = (numeroPanel == 1) ? panelQuest1 : panelQuest2;
        TextMeshProUGUI txtTitulo = (numeroPanel == 1) ? textoTitulo1 : textoTitulo2;
        TextMeshProUGUI txtContenido = (numeroPanel == 1) ? textoContenido1 : textoContenido2;
        CanvasGroup cg = (numeroPanel == 1) ? canvasGroup1 : canvasGroup2;

        txtTitulo.text = titulo;
        txtContenido.text = contenido;
        panel.SetActive(true);

        LeanTween.cancel(panel);
        cg.alpha = 0f;
        LeanTween.value(panel, 0f, 1f, duracionFade)
            .setOnUpdate((float val) => { cg.alpha = val; });
    }

    public void OcultarQuest(int numeroPanel)
    {
        GameObject panel = (numeroPanel == 1) ? panelQuest1 : panelQuest2;
        CanvasGroup cg = (numeroPanel == 1) ? canvasGroup1 : canvasGroup2;

        LeanTween.cancel(panel);
        LeanTween.value(panel, cg.alpha, 0f, duracionFade)
            .setOnUpdate((float val) => { cg.alpha = val; })
            .setOnComplete(() => { panel.SetActive(false); });
    }
}
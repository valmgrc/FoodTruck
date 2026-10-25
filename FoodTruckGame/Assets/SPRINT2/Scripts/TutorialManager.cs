using UnityEngine;
using TMPro;
using DentedPixel;

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
    private bool recetarioAbierto = false;

    public float duracionFade = 0.3f;

    private bool misionMovimientoActiva = true;
    private bool misionRecetarioActiva = false;
    private bool misionCortarActiva = false;
    private bool misionHervirActiva = false;
    private bool misionServirActiva = false;
    private bool misionTipDuplicarActiva = false;

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
        MostrarQuest(1, "Tip", "Puedes moverte alrededor con W, A, S, D");

        Invoke("ActivarMisionRecetario", 1.5f);
    }

    void Update()
    {
        //lo de moverse
        if (misionMovimientoActiva)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                misionMovimientoActiva = false;
                OcultarQuest(1);
            }
        }
        //el recetario
        if (Input.GetKeyDown(KeyCode.N))
        {
            recetarioAbierto = !recetarioAbierto;

            if (panelRecetarioVisual != null)
            {
                panelRecetarioVisual.SetActive(recetarioAbierto);
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


                if (misionRecetarioActiva)
                {
                    misionRecetarioActiva = false;
                    OcultarQuest(2);

                    Invoke("ActivarMisionCortar", 0.5f);
                }
            }
        }
    }

    void ActivarMisionRecetario()
    {
        misionRecetarioActiva = true;
        MostrarQuest(2, "Tip", "Puedes revisar tu recetario con la tecla N");
    }

    void ActivarMisionCortar()
    {
        misionCortarActiva = true;
        MostrarQuest(1, "Preparación", "Dirígete al área de cortar para comenzar a cocinar");

        misionTipDuplicarActiva = true;
        MostrarQuest(2, "Tip", "Recuerda que puedes hacer 2 porciones de tu platillo al duplicar ingredientes");
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

        if (misionTipDuplicarActiva)
        {
            misionTipDuplicarActiva = false;
            OcultarQuest(2);
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
using UnityEngine;

public class VentanaPedidos : MonoBehaviour
{
    public Animator animatorVentana;
    public string nombreTriggerAbrir = "Abrir";
    public bool usarBoolEnVezDeTrigger = false;

    [HideInInspector]
    public bool estaAbierta = false;

    private Outline[] outlines;

    void Awake()
    {
        outlines = GetComponentsInChildren<Outline>(true);
        SetOutlines(false);
    }

    void Start()
    {
        if (animatorVentana == null)
            animatorVentana = GetComponentInParent<Animator>();

        SetOutlines(false);
    }

    private void SetOutlines(bool estado)
    {
        if (outlines == null) return;
        foreach (Outline o in outlines)
        {
            if (o != null) o.enabled = estado;
        }
    }

    public void AbrirVentana()
    {
        if (estaAbierta) return;

        estaAbierta = true;

        if (animatorVentana != null)
        {
            if (usarBoolEnVezDeTrigger)
                animatorVentana.SetBool(nombreTriggerAbrir, true);
            else
                animatorVentana.SetTrigger(nombreTriggerAbrir);
        }

        SetOutlines(false);

        if (ButtonPromptManager.Instance != null)
            ButtonPromptManager.Instance.OcultarPrompts();

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.NotificarVentanaAbierta();
        }
    }

    private void OnMouseEnter()
    {
        if (!estaAbierta && TutorialManager.Instance != null && TutorialManager.Instance.esperandoAperturaVentanas)
        {
            SetOutlines(true);
            if (ButtonPromptManager.Instance != null)
                ButtonPromptManager.Instance.MostrarPrompt("ClickIzq", "ABRIR VENTANA");
        }
    }

    private void OnMouseExit()
    {
        SetOutlines(false);
        if (ButtonPromptManager.Instance != null && TutorialManager.Instance != null && TutorialManager.Instance.esperandoAperturaVentanas)
        {
            ButtonPromptManager.Instance.OcultarPrompts();
        }
    }
}
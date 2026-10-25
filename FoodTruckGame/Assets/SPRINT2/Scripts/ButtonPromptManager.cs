using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonPromptManager : MonoBehaviour
{
    public static ButtonPromptManager Instance;

    public GameObject panelPromptContainer;

    public Sprite spriteTeclaE;
    public Sprite spriteClickIzq;
    public Sprite spriteRueda;

    public GameObject filaPrompt1;
    public Image imagenIcono1;     
    public TextMeshProUGUI textoAccion1;

    public GameObject filaPrompt2;
    public Image imagenIcono2;       
    public TextMeshProUGUI textoAccion2;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        OcultarPrompts();
    }

    public void MostrarPrompt(string tipoTecla, string descripcion)
    {
        if (panelPromptContainer != null) panelPromptContainer.SetActive(true);

        if (filaPrompt1 != null)
        {
            filaPrompt1.SetActive(true);
            AsignarSpriteATextura(tipoTecla, imagenIcono1);
            if (textoAccion1 != null) textoAccion1.text = descripcion;
        }

        if (filaPrompt2 != null) filaPrompt2.SetActive(false);
    }

    public void MostrarDoblePrompt(string tipoTecla1, string desc1, string tipoTecla2, string desc2)
    {
        if (panelPromptContainer != null) panelPromptContainer.SetActive(true);

        if (filaPrompt1 != null)
        {
            filaPrompt1.SetActive(true);
            AsignarSpriteATextura(tipoTecla1, imagenIcono1);
            if (textoAccion1 != null) textoAccion1.text = desc1;
        }

        if (filaPrompt2 != null)
        {
            filaPrompt2.SetActive(true);
            AsignarSpriteATextura(tipoTecla2, imagenIcono2);
            if (textoAccion2 != null) textoAccion2.text = desc2;
        }
    }

    public void OcultarPrompts()
    {
        if (panelPromptContainer != null) panelPromptContainer.SetActive(false);
        if (filaPrompt1 != null) filaPrompt1.SetActive(false);
        if (filaPrompt2 != null) filaPrompt2.SetActive(false);
    }

    private void AsignarSpriteATextura(string tipo, Image imagenUI)
    {
        if (imagenUI == null) return;

        if (tipo == "E")
        {
            imagenUI.gameObject.SetActive(true);
            imagenUI.sprite = spriteTeclaE;
        }
        else if (tipo == "ClickIzq")
        {
            imagenUI.gameObject.SetActive(true);
            imagenUI.sprite = spriteClickIzq;
        }
        else if (tipo == "Rueda")
        {
            imagenUI.gameObject.SetActive(true);
            imagenUI.sprite = spriteRueda;
        }
        else
        {
            imagenUI.gameObject.SetActive(false);
        }
    }
}
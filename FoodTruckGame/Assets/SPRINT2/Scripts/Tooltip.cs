using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DentedPixel;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI titulo;
    public TextMeshProUGUI contenido;
    public LayoutElement layout;
    public int characterWrapLimit;

    private CanvasGroup canvasGroup;
    public float duracionFade = 0.2f;

    private void Awake()
    {

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        LeanTween.cancel(gameObject);
        canvasGroup.alpha = 0f;

        LeanTween.value(gameObject, 0f, 1f, duracionFade)
            .setOnUpdate((float val) => {
                canvasGroup.alpha = val;
            });
    }

    public void SetText(string Contenido, string Titulo = "")
    {
        if (string.IsNullOrEmpty(Titulo))
        {
            titulo.gameObject.SetActive(false);
        }
        else
        {
            titulo.gameObject.SetActive(true);
            titulo.text = Titulo;
        }

        contenido.text = Contenido;

        int tituloLength = titulo.text.Length;
        int contenidoLength = contenido.text.Length;

        layout.enabled = (tituloLength > characterWrapLimit || contenidoLength > characterWrapLimit) ? true : false;
    }

    private void Update()
    {
        if (!Application.isPlaying) return;

        Vector2 position = Input.mousePosition;
        transform.position = position;
    }
}
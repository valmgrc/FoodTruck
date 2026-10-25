using UnityEngine;
using TMPro;
using DentedPixel;

public class EstacionTooltip : MonoBehaviour
{
    public static EstacionTooltip Instance;
    public TextMeshProUGUI textoEstacion;
    private CanvasGroup canvasGroup;
    public float duracionFade = 0.2f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public static void Show(string mensaje)
    {
        if (Instance == null) return;

        Instance.gameObject.SetActive(true);
        Instance.textoEstacion.text = mensaje;

        LeanTween.cancel(Instance.gameObject);
        Instance.canvasGroup.alpha = 0f;
        LeanTween.value(Instance.gameObject, 0f, 1f, Instance.duracionFade)
            .setOnUpdate((float val) => {
                Instance.canvasGroup.alpha = val; 
            });
    }

    public static void Hide()
    {
        if (Instance == null) return;

        LeanTween.cancel(Instance.gameObject);
        LeanTween.value(Instance.gameObject, Instance.canvasGroup.alpha, 0f, Instance.duracionFade) 
            .setOnUpdate((float val) => {
                Instance.canvasGroup.alpha = val; 
            })
            .setOnComplete(() => {
                Instance.gameObject.SetActive(false);
            });
    }
}
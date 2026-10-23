using UnityEngine;
using UnityEngine.EventSystems;
using DentedPixel;


public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    //tooltip texto
    public string Contenido;
    public string Titulo;

    private static LTDescr delay;

    public void OnPointerEnter(PointerEventData eventData)
    {
        delay = LeanTween.delayedCall(0.5f, () =>
        {

            TooltipSystem.Show(Contenido, Titulo);
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(delay.uniqueId);
        TooltipSystem.Hide();
    }
}

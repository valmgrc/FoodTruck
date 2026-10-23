using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;
    public Tooltip tooltip;

    public void Awake()
    {
        current = this;
    }

    public static void Show(string Contenido, string Titulo = "")
    {
        current.tooltip.SetText(Contenido, Titulo);
        current.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        current.tooltip.gameObject.SetActive(false);
    }
}

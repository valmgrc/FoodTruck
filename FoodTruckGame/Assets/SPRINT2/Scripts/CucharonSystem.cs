using UnityEngine;

public class CucharonSystem : MonoBehaviour
{
    public bool tieneSopa = false;
    public GameObject visualSopaCucharon;

    public static bool cucharonEnMano = false;

    void Start()
    {
        if (visualSopaCucharon != null)
        {
            visualSopaCucharon.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        
    }

    public void TomarSopaDeOlla(ControladorOlla ollaTarget)
    {
        if (tieneSopa) return; 

        bool exito = ollaTarget.ExtraerPorcionDeOlla();

        if (exito)
        {
            tieneSopa = true;
            if (visualSopaCucharon != null)
            {
                visualSopaCucharon.SetActive(true);
            }
        }
    }

    public bool VaciarEnVasija()
    {
        if (!tieneSopa) return false;

        tieneSopa = false;
        if (visualSopaCucharon != null)
        {
            visualSopaCucharon.SetActive(false);
        }
        return true;
    }
}
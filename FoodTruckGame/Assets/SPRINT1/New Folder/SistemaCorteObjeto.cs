using UnityEngine;

public class SistemaCorteObjeto : MonoBehaviour
{
    [Header("Configuración")]
    public int clicsNecesarios = 3;
    private int clicsActuales = 0;

    [Header("Objetos")]
    public GameObject objetoEntero;       // La verdura entera que está en la mesa
    public GameObject prefabPicado;       // El prefab de la verdura picada que guardaste en tus carpetas

    // Esta variable la activaremos cuando agarres el cuchillo
    public static bool cuchilloEnMano = false;

    void OnMouseDown()
    {
        // 1. Si NO tienes el cuchillo en la mano, no hace nada
        if (!cuchilloEnMano)
        {
            Debug.Log("Necesitas tener el cuchillo en la mano.");
            return;
        }

        // 2. Si ya está picada, no hace nada
        if (clicsActuales >= clicsNecesarios) return;

        // 3. Suma el clic
        clicsActuales++;
        Debug.Log("Corte " + clicsActuales + " de " + clicsNecesarios);

        // 4. Si llega a los clics necesarios, hace el cambio
        if (clicsActuales >= clicsNecesarios)
        {
            // Creamos la verdura picada exactamente en la posición y rotación de la verdura entera
            if (prefabPicado != null && objetoEntero != null)
            {
                Instantiate(prefabPicado, objetoEntero.transform.position, objetoEntero.transform.rotation);
            }

            // Desactivamos (o destruimos) la verdura entera
            if (objetoEntero != null)
            {
                Destroy(objetoEntero); // O puedes usar objetoEntero.SetActive(false);
            }

            Debug.Log("¡Verdura cortada y generada!");
        }
    }
}
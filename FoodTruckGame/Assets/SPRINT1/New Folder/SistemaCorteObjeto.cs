using UnityEngine;

public class SistemaCorteObjeto : MonoBehaviour
{

    public int clicsNecesarios = 3;
    private int clicsActuales = 0;


    public GameObject objetoEntero;    
    public GameObject prefabPicado;  

    public static bool cuchilloEnMano = false;

    void OnMouseDown()
    {
      
        if (!cuchilloEnMano)
        {
            Debug.Log("Necesitas cuchillo");
            return;
        }


        if (clicsActuales >= clicsNecesarios) return;


        clicsActuales++;
        Debug.Log("Corte " + clicsActuales + " de " + clicsNecesarios);


        if (clicsActuales >= clicsNecesarios)
        {

            if (prefabPicado != null && objetoEntero != null)
            {
                Instantiate(prefabPicado, objetoEntero.transform.position, objetoEntero.transform.rotation);
            }


            if (objetoEntero != null)
            {
                Destroy(objetoEntero);
            }

            Debug.Log("Verdura cortada");
        }
    }
}
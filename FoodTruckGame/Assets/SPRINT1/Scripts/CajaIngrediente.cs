using UnityEngine;

public class CajaIngrediente : MonoBehaviour
{

    public GameObject prefabIngrediente;
    public int cantidadDisponible = 3;

    public GameObject[] objetosVisualesCaja;

    void Start()
    {
        if (cantidadDisponible <= 0)
        {
            cantidadDisponible = 3;
        }
        ActualizarVisualesCaja();
    }

    public GameObject ExtraerIngrediente(Transform puntoMano)
    {
        if (cantidadDisponible > 0 && prefabIngrediente != null)
        {
            cantidadDisponible--;
            ActualizarVisualesCaja();

            GameObject nuevoIngrediente = Instantiate(prefabIngrediente, puntoMano);

            nuevoIngrediente.transform.localPosition = Vector3.zero;
            nuevoIngrediente.transform.localRotation = Quaternion.identity;


            Collider[] colliders = nuevoIngrediente.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }

            Rigidbody rb = nuevoIngrediente.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            return nuevoIngrediente;
        }
        else
        {
            return null;
        }
    }

    void ActualizarVisualesCaja()
    {
        if (objetosVisualesCaja != null && objetosVisualesCaja.Length > 0)
        {
            for (int i = 0; i < objetosVisualesCaja.Length; i++)
            {
                if (objetosVisualesCaja[i] != null)
                {
                    objetosVisualesCaja[i].SetActive(i < cantidadDisponible);
                }
            }
        }
    }
}
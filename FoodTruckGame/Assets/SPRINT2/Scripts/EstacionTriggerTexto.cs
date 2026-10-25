using UnityEngine;

public class EstacionTriggerTexto : MonoBehaviour
{
    public string nombreEstacion = "Área de Cortado";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EstacionTooltip.Show(nombreEstacion);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EstacionTooltip.Hide();
        }
    }
}
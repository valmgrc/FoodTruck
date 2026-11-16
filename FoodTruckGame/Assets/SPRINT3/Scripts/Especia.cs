using UnityEngine;

public class Especia : MonoBehaviour
{
    public CajaEspeciasController.TipoEspecie tipoEspecie;
    public CajaEspeciasController controladorPrincipal;

    private void Start()
    {
        if (controladorPrincipal == null)
            controladorPrincipal = GetComponentInParent<CajaEspeciasController>();
    }

    public void CargarEnCuchara()
    {
        if (ObjectSelected.objetoAgarradoActual != null && controladorPrincipal != null)
        {
            controladorPrincipal.CargarEspecieDesdeCompartimento(tipoEspecie, ObjectSelected.objetoAgarradoActual);
        }
    }
}
using UnityEngine;

public class CajaEspeciasController : MonoBehaviour
{
    public enum TipoEspecie { Sal, Pimienta, Azucar }

    public void CargarEspecieDesdeCompartimento(TipoEspecie especie, ObjectSelected cucharaObj)
    {
        if (cucharaObj == null) return;

        CucharitaEspecias cucharaSys = cucharaObj.GetComponent<CucharitaEspecias>();
        if (cucharaSys != null)
        {

            if (especie == TipoEspecie.Sal)
            {
                cucharaSys.CargarSal();
            }
            else if (especie == TipoEspecie.Pimienta)
            {
            }
            else if (especie == TipoEspecie.Azucar)
            {
                
            }
        }
    }
}
using UnityEngine;

public class IngredienteCocinable : MonoBehaviour
{
    public enum TipoIngrediente { Daikon, Cebollin }
    public TipoIngrediente tipo;

    public void EntrarAOlla()
    {
        Destroy(gameObject);
    }
}
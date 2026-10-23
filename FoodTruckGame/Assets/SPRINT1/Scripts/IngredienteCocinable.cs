using UnityEngine;

public class IngredienteCocinable : MonoBehaviour
{
    public enum TipoIngrediente { Daikon, Cebollin, Mijo }
    public TipoIngrediente tipo;

    public void EntrarAOlla()
    {
        Destroy(gameObject);
    }
}
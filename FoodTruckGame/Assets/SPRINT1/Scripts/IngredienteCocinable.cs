using UnityEngine;

public class IngredienteCocinable : MonoBehaviour
{
    public enum TipoIngrediente { Daikon, Cebollin, Mijo }
    public TipoIngrediente tipo;

    private bool yaEntro = false;

    public bool IntentarEntrarAOlla()
    {
        if (yaEntro) return false;
        yaEntro = true;
        Destroy(gameObject);
        return true;
    }
}
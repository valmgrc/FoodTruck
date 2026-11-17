using UnityEngine;

public class MonedasController : MonoBehaviour
{
    public int valorMonedas = 50;

    private void OnMouseDown()
    {

        Destroy(gameObject);
    }
}
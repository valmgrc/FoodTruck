using UnityEngine;

public class VasijaTelaController : MonoBehaviour
{

    public GameObject modeloVasijaDestapada; 
    public GameObject modeloVasijaTapada;  


    public bool estaTapada = false;

    void Start()
    {
        ActualizarVisuales();
    }

    public void ActualizarVisuales()
    {
        if (modeloVasijaDestapada != null) modeloVasijaDestapada.SetActive(!estaTapada);
        if (modeloVasijaTapada != null) modeloVasijaTapada.SetActive(estaTapada);
    }

    public void ColocarTela()
    {
        if (estaTapada) return;

        if (ObjectSelected.hayObjetoAgarrado && ObjectSelected.objetoAgarradoActual != null)
        {
            GameObject objetoMano = ObjectSelected.objetoAgarradoActual.gameObject;

            if (objetoMano.CompareTag("Tela") || objetoMano.name.Contains("Tela"))
            {
                ObjectSelected.hayObjetoAgarrado = false;
                ObjectSelected.objetoAgarradoActual = null;

                Destroy(objetoMano);

                estaTapada = true;
                ActualizarVisuales();

                if (ButtonPromptManager.Instance != null)
                {
                    ButtonPromptManager.Instance.OcultarPrompts();
                }

            }
        }
    }
}
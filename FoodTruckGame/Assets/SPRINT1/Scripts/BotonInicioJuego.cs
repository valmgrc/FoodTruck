using UnityEngine;
using UnityEngine.SceneManagement; 

public class BotonInicioJuego : MonoBehaviour
{
    public void CargarEscenaPorNombre(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}

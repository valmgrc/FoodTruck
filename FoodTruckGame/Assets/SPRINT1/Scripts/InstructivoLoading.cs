using UnityEngine;
using UnityEngine.SceneManagement; 

public class InstructivoLoading : MonoBehaviour
{
  
    public void CargarEscenaJuego(string nombreDeLaEscena)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(nombreDeLaEscena);
    }
}
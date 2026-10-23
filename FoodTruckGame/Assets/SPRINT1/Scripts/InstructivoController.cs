using UnityEngine;

public class InstructivoController : MonoBehaviour
{

    public GameObject instructivoPanel;
    

    public MonoBehaviour scriptMovimientoJugador;

    void Start()
    {
        if (instructivoPanel != null)
        {
            instructivoPanel.SetActive(true);
            PausarJuegoParaInstructivo();
        }
    }

    void PausarJuegoParaInstructivo()
    {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;


        if (scriptMovimientoJugador != null)
        {
            scriptMovimientoJugador.enabled = false;
        }
    }

    public void CerrarInstructivo()
    {
        if (instructivoPanel != null)
        {
            instructivoPanel.SetActive(false);
        }


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        Time.timeScale = 1f;

        if (scriptMovimientoJugador != null)
        {
            scriptMovimientoJugador.enabled = true;
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class IrInicio : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void IrAEscenaPorNombre(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}

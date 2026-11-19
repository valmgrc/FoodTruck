using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

public class TiempoCerrar : MonoBehaviour
{
    public float tiempoRestanteServicio = 240f; 
    public TextMeshProUGUI textoCronometro;

    public string nombreEscenaFinal = "FinPrueba"; 

    [HideInInspector] public bool tiempoAgotado = false;

    void Update()
    {
        if (tiempoAgotado) return;

        if (tiempoRestanteServicio > 0)
        {
            tiempoRestanteServicio -= Time.deltaTime;
            ActualizarTextoUI();
        }
        else
        {
            tiempoRestanteServicio = 0;
            tiempoAgotado = true;
            ActualizarTextoUI();
            DispararFinDeServicio();
        }
    }

    void ActualizarTextoUI()
    {
        if (textoCronometro != null)
        {
            int minutos = Mathf.FloorToInt(tiempoRestanteServicio / 60);
            int segundos = Mathf.FloorToInt(tiempoRestanteServicio % 60);
            textoCronometro.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }

    void DispararFinDeServicio()
    {
        if (textoCronometro != null)
        {
            textoCronometro.gameObject.SetActive(false);
        }
        SceneManager.LoadScene(nombreEscenaFinal);
    }
}
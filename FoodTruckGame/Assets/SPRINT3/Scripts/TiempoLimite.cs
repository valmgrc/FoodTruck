using UnityEngine;
using TMPro;

public class TiempoLimite : MonoBehaviour
{
    public float tiempoRestante = 60f; 
    public TextMeshProUGUI textoCronometro;

    [HideInInspector] public bool tiempoAgotado = false;

    void Update()
    {
        if (tiempoAgotado) return;

        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarTextoUI();
        }
        else
        {
            tiempoRestante = 0;
            tiempoAgotado = true;
            ActualizarTextoUI();
            DispararFinDePreparacion();
        }
    }

    void ActualizarTextoUI()
    {
        if (textoCronometro != null)
        {
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            textoCronometro.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }

    void DispararFinDePreparacion()
    {
        if (textoCronometro != null)
        {
            textoCronometro.gameObject.SetActive(false);
        }

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.IniciarTransicionPorTiempo();
        }
    }
}
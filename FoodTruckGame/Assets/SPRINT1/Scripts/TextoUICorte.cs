using UnityEngine;
using System.Collections;
using TMPro;

public class TextoUICorte : MonoBehaviour
{
    public static TextoUICorte Instancia;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
    }

    public void MostrarAviso(TextMeshProUGUI texto, string mensaje, float tiempoEspera, float tiempoFade)
    {
        StopAllCoroutines(); 
        StartCoroutine(RutinaFadeTexto(texto, mensaje, tiempoEspera, tiempoFade));
    }

    IEnumerator RutinaFadeTexto(TextMeshProUGUI texto, string mensaje, float tiempoEspera, float tiempoFade)
    {
        texto.text = mensaje;
        texto.gameObject.SetActive(true);


        Color colorInicial = texto.color;
        colorInicial.a = 1f;
        texto.color = colorInicial;

        yield return new WaitForSeconds(tiempoEspera);

        float tiempo = 0f;
        while (tiempo < tiempoFade)
        {
            tiempo += Time.deltaTime;
            float alfa = Mathf.Lerp(1f, 0f, tiempo / tiempoFade);

            Color c = texto.color;
            c.a = alfa;
            texto.color = c;

            yield return null;
        }

        Color colorFinal = texto.color;
        colorFinal.a = 0f;
        texto.color = colorFinal;
        texto.gameObject.SetActive(false);
    }
}
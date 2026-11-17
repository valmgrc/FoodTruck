using UnityEngine;
using System.Collections;

public class PlatoLargoController : MonoBehaviour
{
    public Transform puntoPescado;
    public GameObject planoVino;
    public Transform puntoVinoVacio;
    public Transform puntoVinoLleno;
    public float duracionVertido = 4f;

    public bool tienePescado = false;
    public bool tieneVino = false;
    public bool estaVertiendo = false;
    public GameObject pescadoColocadoActual;

    public bool tieneSal = false;

    void Start()
    {
        if (planoVino != null)
        {
            planoVino.SetActive(false);
        }
    }

    public void ColocarPescado(ObjectSelected pescadoSeleccionado)
    {
        if (tienePescado || pescadoSeleccionado == null) return;

        pescadoSeleccionado.Drop();

        GameObject objPescado = pescadoSeleccionado.gameObject;
        objPescado.transform.SetParent(puntoPescado);
        objPescado.transform.localPosition = Vector3.zero;
        objPescado.transform.localRotation = Quaternion.identity;

        Rigidbody rb = objPescado.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.detectCollisions = false;
        }

        pescadoSeleccionado.puedoInteractuar = false;
        Collider col = objPescado.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        pescadoColocadoActual = objPescado;
        tienePescado = true;

        if (ButtonPromptManager.Instance != null)
        {
            ButtonPromptManager.Instance.OcultarPrompts();
        }
    }

    public void IniciarVertidoVino(ObjectSelected botellaVino)
    {
        if (!tienePescado || tieneVino || estaVertiendo || botellaVino == null) return;
        StartCoroutine(RutinaVertidoVino(botellaVino.gameObject));
    }

    private IEnumerator RutinaVertidoVino(GameObject botella)
    {
        estaVertiendo = true;

        if (ButtonPromptManager.Instance != null)
        {
            ButtonPromptManager.Instance.OcultarPrompts();
        }

        Quaternion rotacionOriginal = botella.transform.localRotation;
        Quaternion rotacionInclinada = rotacionOriginal * Quaternion.Euler(115f, 0f, 0f);

        ParticleSystem particulasLiquido = botella.GetComponentInChildren<ParticleSystem>();

        float tInclinacion = 0f;
        while (tInclinacion < 0.4f)
        {
            tInclinacion += Time.deltaTime;
            botella.transform.localRotation = Quaternion.Slerp(rotacionOriginal, rotacionInclinada, tInclinacion / 0.4f);
            yield return null;
        }

        if (particulasLiquido != null) particulasLiquido.Play();

        if (planoVino != null && puntoVinoVacio != null)
        {
            planoVino.transform.position = puntoVinoVacio.position;
            planoVino.SetActive(true);
        }

        float tiempoRestante = duracionVertido - 0.8f;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoRestante)
        {
            tiempoTranscurrido += Time.deltaTime;
            float porcentaje = Mathf.Clamp01(tiempoTranscurrido / tiempoRestante);

            if (planoVino != null && puntoVinoVacio != null && puntoVinoLleno != null)
            {
                planoVino.transform.position = Vector3.Lerp(puntoVinoVacio.position, puntoVinoLleno.position, porcentaje);
            }

            yield return null;
        }

        if (planoVino != null && puntoVinoLleno != null)
        {
            planoVino.transform.position = puntoVinoLleno.position;
        }

        if (particulasLiquido != null) particulasLiquido.Stop();

        float tRegreso = 0f;
        while (tRegreso < 0.4f)
        {
            tRegreso += Time.deltaTime;
            botella.transform.localRotation = Quaternion.Slerp(rotacionInclinada, rotacionOriginal, tRegreso / 0.4f);
            yield return null;
        }

        botella.transform.localRotation = rotacionOriginal;
        tieneVino = true;
        estaVertiendo = false;
    }

    public void AplicarSal(ObjectSelected cucharaObj)
    {
        if (!tienePescado || !tieneVino || tieneSal || cucharaObj == null) return;

        CucharitaEspecias cucharaEspecias = cucharaObj.GetComponent<CucharitaEspecias>();
        if (cucharaEspecias != null && cucharaEspecias.tieneSalCargada)
        {
            StartCoroutine(RutinaAplicarSal(cucharaEspecias));
        }
    }

    private IEnumerator RutinaAplicarSal(CucharitaEspecias cuchara)
    {
        yield return StartCoroutine(cuchara.EspolvorearSalRutina());

        tieneSal = true;
    }

}
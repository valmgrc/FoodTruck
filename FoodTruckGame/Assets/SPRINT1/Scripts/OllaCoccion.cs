using System.Collections;
using UnityEngine;
using TMPro;

public class OllaCoccion : MonoBehaviour
{
    public GameObject textoTimerObjeto;
    public TextMeshProUGUI textoTimer;

    public Transform puntoDepositoOlla;
    public float tiempoDeHervor = 12f;

    public GameObject aguaCruda;
    public GameObject sopaLista;

    private bool cocinando = false;
    private bool sopaTerminada = false;
    private GameObject verduraHervida;

    public bool SopaTerminada => sopaTerminada;

    void Start()
    {
        if (textoTimerObjeto != null) textoTimerObjeto.SetActive(false);
        if (sopaLista != null) sopaLista.SetActive(false);
        if (aguaCruda != null) aguaCruda.SetActive(true);
    }

    public void RecibirVerduras(GameObject bowl)
    {
        if (cocinando) return;
        if (sopaTerminada) return;

        Daikon_Cortar verduraEnBowl = bowl.GetComponentInChildren<Daikon_Cortar>();

        if (verduraEnBowl != null)
        {
            verduraEnBowl.transform.SetParent(puntoDepositoOlla);
            verduraEnBowl.transform.localPosition = Vector3.zero;
            verduraEnBowl.gameObject.layer = 0;
            foreach (Transform hijo in verduraEnBowl.GetComponentsInChildren<Transform>())
            {
                hijo.gameObject.layer = 0;
            }

            verduraHervida = verduraEnBowl.gameObject;
            StartCoroutine(Cocinar());
        }
    }

    private IEnumerator Cocinar()
    {
        cocinando = true;

        if (textoTimerObjeto != null) textoTimerObjeto.SetActive(true);

        float tiempoRestante = tiempoDeHervor;

        while (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;

            int segundos = Mathf.CeilToInt(tiempoRestante);
            if (textoTimer != null) textoTimer.text = "0:" + segundos.ToString("D2");

            yield return null;
        }

        if (textoTimerObjeto != null) textoTimerObjeto.SetActive(false);
        if (aguaCruda != null) aguaCruda.SetActive(false);
        if (sopaLista != null) sopaLista.SetActive(true);

        cocinando = false;
        sopaTerminada = true;
    }

    public GameObject ExtraerSopa()
    {
        if (sopaTerminada)
        {
            if (verduraHervida != null)
            {
                GameObject porcion = verduraHervida;
                verduraHervida = null;
                sopaTerminada = false;

                if (aguaCruda != null) aguaCruda.SetActive(true);
                if (sopaLista != null) sopaLista.SetActive(false);

                return porcion;
            }
        }
        return null;
    }
}
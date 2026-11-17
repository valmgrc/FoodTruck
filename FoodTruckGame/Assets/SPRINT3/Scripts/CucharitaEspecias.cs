using UnityEngine;
using System.Collections;

public class CucharitaEspecias : MonoBehaviour
{
    public static bool cucharaEspeciasEnMano = false;

    public GameObject objetoSalEnCuchara; 
    public ParticleSystem particulasSal; 
    public bool tieneSalCargada = false;

    void Start()
    {
        if (objetoSalEnCuchara != null)
            objetoSalEnCuchara.SetActive(false);

        if (particulasSal != null)
            particulasSal.Stop();
    }

    public void OnGrab()
    {
        cucharaEspeciasEnMano = true;
    }

    public void OnDrop()
    {
        cucharaEspeciasEnMano = false;
    }

    public void CargarSal()
    {
        if (tieneSalCargada) return;

        tieneSalCargada = true;
        if (objetoSalEnCuchara != null)
        {
            objetoSalEnCuchara.SetActive(true);
        }
    }


    public IEnumerator EspolvorearSalRutina()
    {
        if (!tieneSalCargada) yield break;

        tieneSalCargada = false;
        if (objetoSalEnCuchara != null)
        {
            objetoSalEnCuchara.SetActive(false); 
        }

        if (particulasSal != null)
        {
            particulasSal.Play();
            yield return new WaitForSeconds(1.5f);
            particulasSal.Stop();
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneradorClientes : MonoBehaviour
{
    public GameObject prefabCliente;

    public Transform puntoSpawn1;
    public Transform puntoDestino1;
    [HideInInspector] public bool punto1Ocupado = false;

    public Transform puntoSpawn2;
    public Transform puntoDestino2;
    [HideInInspector] public bool punto2Ocupado = false;


    public float tiempoMinimoSpawn = 5f;
    public float tiempoMaximoSpawn = 10f; 

    void Start()
    {
        StartCoroutine(RutinaBucleClientes());
    }

    IEnumerator RutinaBucleClientes()
    {
        yield return new WaitForSeconds(5f);

        while (true)
        {
            IntentarSpawnearClientes();

            float tiempoEspera = Random.Range(tiempoMinimoSpawn, tiempoMaximoSpawn);
            yield return new WaitForSeconds(tiempoEspera);
        }
    }

    void IntentarSpawnearClientes()
    {
        List<int> puntosDisponibles = new List<int>();

        if (!punto1Ocupado) puntosDisponibles.Add(1);
        if (!punto2Ocupado) puntosDisponibles.Add(2);

        if (puntosDisponibles.Count == 0) return; 

        int cantidadASpawnear = 1;
        if (puntosDisponibles.Count == 2 && Random.value > 0.5f)
        {
            cantidadASpawnear = 2;
        }

        for (int i = 0; i < cantidadASpawnear; i++)
        {
            if (puntosDisponibles.Count == 0) break;

            int indiceElegido = Random.Range(0, puntosDisponibles.Count);
            int numeroPunto = puntosDisponibles[indiceElegido];
            puntosDisponibles.RemoveAt(indiceElegido);

            if (numeroPunto == 1)
            {
                punto1Ocupado = true;
                CrearCliente(puntoSpawn1, puntoDestino1, 1);
            }
            else if (numeroPunto == 2)
            {
                punto2Ocupado = true;
                CrearCliente(puntoSpawn2, puntoDestino2, 2);
            }
        }
    }

    void CrearCliente(Transform spawn, Transform destino, int idPunto)
    {
        if (prefabCliente == null || spawn == null || destino == null) return;

        GameObject nuevoClienteObj = Instantiate(prefabCliente, spawn.position, Quaternion.identity);
        ClienteController cliente = nuevoClienteObj.GetComponent<ClienteController>();

        if (cliente != null)
        {
            cliente.InicializarCliente(destino.position);

            cliente.idPuntoAsignado = idPunto;
            cliente.generadorRef = this;
        }
    }

    public void LiberarPunto(int idPunto)
    {
        if (idPunto == 1) punto1Ocupado = false;
        if (idPunto == 2) punto2Ocupado = false;
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneradorClientes : MonoBehaviour
{
    public GameObject prefabCliente;

    public Transform puntoSpawn1;
    public Transform puntoDestino1;
    public Transform puntoMonedas1;
    [HideInInspector] public bool punto1Ocupado = false;

    public Transform puntoSpawn2;
    public Transform puntoDestino2;
    public Transform puntoMonedas2;
    [HideInInspector] public bool punto2Ocupado = false;


    public float tiempoMinimoDescanso = 10f;

    public float tiempoMaximoDescanso = 18f;

    public bool soloUnClienteALaVez = true;

    private float tiempoParaSiguienteCliente = 5f; 

    void Update()
    {

        if (soloUnClienteALaVez && (punto1Ocupado || punto2Ocupado))
        {
            return;
        }
        if (punto1Ocupado && punto2Ocupado)
        {
            return;
        }

        if (tiempoParaSiguienteCliente > 0f)
        {
            tiempoParaSiguienteCliente -= Time.deltaTime;
        }
        else
        {
            IntentarSpawnearCliente();

            tiempoParaSiguienteCliente = Random.Range(tiempoMinimoDescanso, tiempoMaximoDescanso);
        }
    }

    void IntentarSpawnearCliente()
    {
        List<int> puntosDisponibles = new List<int>();

        if (!punto1Ocupado) puntosDisponibles.Add(1);
        if (!punto2Ocupado) puntosDisponibles.Add(2);

        if (puntosDisponibles.Count == 0) return;

        int indiceElegido = Random.Range(0, puntosDisponibles.Count);
        int numeroPunto = puntosDisponibles[indiceElegido];

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

        tiempoParaSiguienteCliente = Random.Range(tiempoMinimoDescanso, tiempoMaximoDescanso);
    }
}
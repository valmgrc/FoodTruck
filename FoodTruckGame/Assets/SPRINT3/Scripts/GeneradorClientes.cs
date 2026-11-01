using UnityEngine;

public class GeneradorClientes : MonoBehaviour
{
    public GameObject prefabCliente;

    //npc1
    public Transform puntoSpawn1;
    public Transform puntoDestino1;
    public float tiempoEsperaNPC1 = 2f;

    //Npc2
    public Transform puntoSpawn2;
    public Transform puntoDestino2;
    public float tiempoEsperaNPC2 = 10f;

    private bool npc1Spawned = false;
    private bool npc2Spawned = false;
    private float temporizador = 0f;

    void Update()
    {
        temporizador += Time.deltaTime;

        if (!npc1Spawned && temporizador >= tiempoEsperaNPC1)
        {
            SpawnearCliente(puntoSpawn1, puntoDestino1);
            npc1Spawned = true;
        }

        if (!npc2Spawned && temporizador >= tiempoEsperaNPC2)
        {
            SpawnearCliente(puntoSpawn2, puntoDestino2);
            npc2Spawned = true;
        }
    }

    void SpawnearCliente(Transform spawn, Transform destino)
    {
        if (prefabCliente == null || spawn == null || destino == null) return;

        GameObject nuevoClienteObj = Instantiate(prefabCliente, spawn.position, Quaternion.identity);

        ClienteController cliente = nuevoClienteObj.GetComponent<ClienteController>();
        if (cliente != null)
        {
            cliente.InicializarCliente(destino.position);
        }
    }
}
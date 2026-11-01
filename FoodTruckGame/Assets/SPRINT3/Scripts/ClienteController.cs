using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClienteController : MonoBehaviour
{
    //mov camara
    public float velocidad = 3f;
    private Vector3 puntoDestino;
    private bool estaCaminando = false;

    public GameObject panelBurbujaPedido;
    public Image imagenPlatillo;
    public Image imagenContenedor;
    public TextMeshProUGUI textoContador;
    public Vector3 offsetPantalla = new Vector3(0f, 80f, 0f);

    public float tiempoMaximo = 30f;
    private float tiempoRestante;
    private bool pedidoActivo = false;

    public Sprite[] listaPlatillos;
    public Sprite spriteCaja;
    public Sprite spriteBowl;

    public void InicializarCliente(Vector3 destino)
    {
        puntoDestino = destino;
        estaCaminando = true;

        if (panelBurbujaPedido != null)
            panelBurbujaPedido.SetActive(false);
    }

    void Update()
    {
        if (estaCaminando)
        {
            transform.position = Vector3.MoveTowards(transform.position, puntoDestino, velocidad * Time.deltaTime);

            if (Vector3.Distance(transform.position, puntoDestino) < 0.1f)
            {
                estaCaminando = false;
                LlegarAlPuesto();
            }
        }

        if (pedidoActivo)
        {
            if (panelBurbujaPedido != null && Camera.main != null)
            {
                Vector3 posicionPantalla = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
                panelBurbujaPedido.transform.position = posicionPantalla + offsetPantalla;
            }

            tiempoRestante -= Time.deltaTime;
            if (textoContador != null)
            {
                textoContador.text = Mathf.Ceil(tiempoRestante).ToString() + "s";
            }

            if (tiempoRestante <= 0)
            {
                ClienteSeVaEnojado();
            }
        }
    }

    void LlegarAlPuesto()
    {
        if (panelBurbujaPedido != null)
            panelBurbujaPedido.SetActive(true);

        GenerarPedidoAleatorio();

        tiempoRestante = tiempoMaximo;
        pedidoActivo = true;
    }

    void GenerarPedidoAleatorio()
    {
        if (listaPlatillos != null && listaPlatillos.Length > 0)
        {
            int indexPlatillo = Random.Range(0, listaPlatillos.Length);
            if (imagenPlatillo != null) imagenPlatillo.sprite = listaPlatillos[indexPlatillo];
        }

        bool esParaLlevar = Random.value > 0.5f;
        if (imagenContenedor != null)
        {
            imagenContenedor.sprite = esParaLlevar ? spriteCaja : spriteBowl;
        }
    }

    public void EntregarPedidoCorrecto()
    {
        pedidoActivo = false;
        if (panelBurbujaPedido != null) panelBurbujaPedido.SetActive(false);
        Destroy(gameObject, 0.5f);
    }

    void ClienteSeVaEnojado()
    {
        pedidoActivo = false;
        if (panelBurbujaPedido != null) panelBurbujaPedido.SetActive(false);
        Destroy(gameObject);
    }
}
using UnityEngine;
using StarterAssets;

public class EstacionesManager : MonoBehaviour
{
    [Header("Referencias del Jugador")]
    public GameObject jugadorFPS;
    public Transform camaraPrincipal;

    [Header("Puntos de las Estaciones")]
    public Transform[] puntosEstaciones;
    public float tiempoTransicion = 0.4f; // Ajustado a 0.4 segundos para que sea un enfoque veloz y satisfactorio

    [Header("Límites de Mirada en la Estación")]
    public float limiteHorizontal = 45f; // Cuántos grados puede mirar a los lados en la mesa
    public float limiteVertical = 30f;   // Cuántos grados puede mirar arriba y abajo en la mesa

    [Header("Referencia al Sistema de Agarre")]
    public SistemaInteraccion sistemaInteraccion;

    private int estacionActual = 0;
    private bool estaEnEstacion = false;
    private bool estaCercaDeAlgunaMesa = false;
    private int mesaDetectada = -1;

    private Vector3 posicionOriginalJugador;
    private Quaternion rotacionOriginalCamara;

    private float cronometroTransicion = 0f;
    private Vector3 posicionInicioCamara;
    private Quaternion rotacionInicioCamara;
    private bool haciendoTransicion = false;

    // Variables para el control de la mirada limitada
    private float rotacionMesaX = 0f;
    private float rotacionMesaY = 0f;

    void Update()
    {
        // Activar/Desactivar con E
        if (Input.GetKeyDown(KeyCode.E) && (estaCercaDeAlgunaMesa || estaEnEstacion))
        {
            if (!estaEnEstacion)
            {
                estacionActual = mesaDetectada;
                EntrarAEstacion();
            }
            else
            {
                SalirDeEstacion();
            }
        }

        if (estaEnEstacion)
        {
            // Cambiar de estación con A y D
            if (!haciendoTransicion)
            {
                if (Input.GetKeyDown(KeyCode.A)) CambiarEstacion(-1);
                if (Input.GetKeyDown(KeyCode.D)) CambiarEstacion(1);
            }

            Transform destino = puntosEstaciones[estacionActual];

            // 1 y 2. Control de la transición/zoom rápido hacia el frente
            if (haciendoTransicion)
            {
                cronometroTransicion += Time.deltaTime / tiempoTransicion;

                camaraPrincipal.position = Vector3.Lerp(posicionInicioCamara, destino.position, cronometroTransicion);
                camaraPrincipal.rotation = Quaternion.Slerp(rotacionInicioCamara, destino.rotation, cronometroTransicion);

                if (cronometroTransicion >= 1f)
                {
                    haciendoTransicion = false;
                    // Inicializamos los ángulos de mirada limitada basados en la rotación fija del punto
                    rotacionMesaX = 0f;
                    rotacionMesaY = 0f;
                }
            }
            else
            {
                // La cámara se mantiene firme en la mesa
                camaraPrincipal.position = destino.position;

                // 3. Mirada libre limitada (Estilo Cooking Simulator) para interactuar
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                rotacionMesaX += mouseX * 2f; // Sensibilidad local básica
                rotacionMesaY -= mouseY * 2f;

                // Ponemos límites estrictos para no girar 360 grados
                rotacionMesaX = Mathf.Clamp(rotacionMesaX, -limiteHorizontal, limiteHorizontal);
                rotacionMesaY = Mathf.Clamp(rotacionMesaY, -limiteVertical, limiteVertical);

                // Aplicamos la rotación limitada sumada a la dirección base del Punto de la estación
                camaraPrincipal.rotation = destino.rotation * Quaternion.Euler(rotacionMesaY, rotacionMesaX, 0f);
            }
        }
    }

    void EntrarAEstacion()
    {
        estaEnEstacion = true;
        haciendoTransicion = true;
        cronometroTransicion = 0f;

        posicionOriginalJugador = jugadorFPS.transform.position;
        rotacionOriginalCamara = camaraPrincipal.rotation;

        posicionInicioCamara = camaraPrincipal.position;
        rotacionInicioCamara = camaraPrincipal.rotation;

        // Apagamos los scripts del Starter Assets para que la cámara y el cuerpo queden bajo nuestro control lineal
        SetControlJugador(false);
    }

    void CambiarEstacion(int direccion)
    {
        if (sistemaInteraccion != null)
        {
            sistemaInteraccion.Invoke("SoltarObjeto", 0f);
        }

        estacionActual += direccion;
        if (estacionActual >= puntosEstaciones.Length) estacionActual = 0;
        if (estacionActual < 0) estacionActual = puntosEstaciones.Length - 1;

        // Iniciamos nueva transición veloz a la siguiente mesa
        haciendoTransicion = true;
        cronometroTransicion = 0f;
        posicionInicioCamara = camaraPrincipal.position;
        rotacionInicioCamara = camaraPrincipal.rotation;
    }

    void SalirDeEstacion()
    {
        estaEnEstacion = false;
        haciendoTransicion = false;

        // Devolvemos la cámara al cuerpo y restauramos la mirada original
        camaraPrincipal.position = camaraPrincipal.parent.position + new Vector3(0, 0.6f, 0);
        camaraPrincipal.rotation = rotacionOriginalCamara;
        jugadorFPS.transform.position = posicionOriginalJugador;

        SetControlJugador(true);
    }

    void SetControlJugador(bool activar)
    {
        // Apaga o prende limpiamente los componentes del Starter Assets
        if (jugadorFPS.GetComponentInChildren<FirstPersonController>() != null)
            jugadorFPS.GetComponentInChildren<FirstPersonController>().enabled = activar;

        if (jugadorFPS.GetComponentInChildren<UnityEngine.InputSystem.PlayerInput>() != null)
            jugadorFPS.GetComponentInChildren<UnityEngine.InputSystem.PlayerInput>().enabled = activar;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<FirstPersonController>() != null || other.CompareTag("Player"))
        {
            estaCercaDeAlgunaMesa = true;

            // Asignación automática infalible por nombre de objeto
            if (gameObject.name.Contains("Cortar")) mesaDetectada = 0;
            if (gameObject.name.Contains("Hervir")) mesaDetectada = 1;
            if (gameObject.name.Contains("Servir")) mesaDetectada = 2;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<FirstPersonController>() != null || other.CompareTag("Player"))
        {
            estaCercaDeAlgunaMesa = false;
            mesaDetectada = -1;
        }
    }
}

using UnityEngine;

public class SistemaInteraccion : MonoBehaviour
{
    [Header("Configuracion de Alcance")]
    public float distanciaAlcance = 3f; // Que tan lejos estira la mano el jugador
    public Transform zonaSostener;       // El punto vacio frente a la camara donde flotará el prop

    private GameObject objetoSostenido;
    private Rigidbody rbObjeto;
    private Camera camaraJugador;

    void Start()
    {
        camaraJugador = Camera.main;
    }

    void Update()
    {
        // Si presionamos Click Izquierdo del Mouse
        if (Input.GetMouseButtonDown(0))
        {
            if (objetoSostenido == null)
            {
                IntentarAgarrar();
            }
            else
            {
                SoltarObjeto();
            }
        }

        // Si tenemos un objeto en la mano, lo movemos con nosotros
        if (objetoSostenido != null)
        {
            MoverObjetoSostenido();
        }
    }

    void IntentarAgarrar()
    {
        // Disparamos un laser invisible desde el centro de la camara (donde esta el puntito blanco)
        Ray rayo = camaraJugador.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit golpe;

        if (Physics.Raycast(rayo, out golpe, distanciaAlcance))
        {
            // Si el laser choca con algo que tenga la etiqueta "Interactuable"
            if (golpe.collider.CompareTag("Interactuable"))
            {
                objetoSostenido = golpe.collider.gameObject;
                rbObjeto = objetoSostenido.GetComponent<Rigidbody>();

                if (rbObjeto != null)
                {
                    rbObjeto.useGravity = false; // Quitamos gravedad para que no pese en la mano
                    rbObjeto.isKinematic = true; // Desactivamos colisiones fisicas bruscas mientras lo tenemos
                }

                // Emparentamos el objeto para que se mueva exactamente con la camara
                objetoSostenido.transform.SetParent(zonaSostener);
            }
        }
    }

    void MoverObjetoSostenido()
    {
        // El objeto viaja suavemente hacia la posicion de la "zonaSostener" usando su pivote de Blender
        objetoSostenido.transform.position = zonaSostener.position;
        objetoSostenido.transform.rotation = zonaSostener.rotation;
    }

    void SoltarObjeto()
    {
        if (rbObjeto != null)
        {
            rbObjeto.useGravity = true;  // Le devolvemos la gravedad
            rbObjeto.isKinematic = false;
        }

        objetoSostenido.transform.SetParent(null); // Lo despegamos de la camara
        objetoSostenido = null;
    }
}

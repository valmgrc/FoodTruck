using UnityEngine;
using System.Collections;

public class EntregaComidaCliente : MonoBehaviour
{
    public bool esperandoPedido = true;

    public void EntregarPlatillo(GameObject objetoEnMano)
    {
        if (!esperandoPedido || objetoEnMano == null) return;

        ControladorBowl bowl = objetoEnMano.GetComponent<ControladorBowl>();

        if (bowl != null && bowl.estaLleno)
        {
            ObjectSelected.hayObjetoAgarrado = false;
            Destroy(objetoEnMano);

            StartCoroutine(RutinaClienteSeVa());
        }
        else
        {
            Debug.Log("ñao ñao comida incorrecta");
        }
    }

    IEnumerator RutinaClienteSeVa()
    {
        esperandoPedido = false;

        float tiempoRotacion = 0.5f;
        Quaternion rotacionInicial = transform.rotation;
        Quaternion rotacionFinal = transform.rotation * Quaternion.Euler(0, 180, 0); //se va girando :D
        float elapsed = 0f;

        while (elapsed < tiempoRotacion)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(rotacionInicial, rotacionFinal, elapsed / tiempoRotacion);
            yield return null;
        }

        float tiempoCaminata = 3f;
        elapsed = 0f;
        while (elapsed < tiempoCaminata)
        {
            elapsed += Time.deltaTime;
            transform.Translate(Vector3.forward * (2f * Time.deltaTime));
            yield return null;
        }

        Destroy(gameObject);
    }
}
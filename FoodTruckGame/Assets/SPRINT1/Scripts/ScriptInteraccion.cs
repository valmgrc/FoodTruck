using System.Collections;
using UnityEngine;

public class ScriptInteraccion : MonoBehaviour
{

    public float distanciaAlcance = 3f;
    public Transform zonaSostener;

 
    public EnfoqueEstacion focusManager;

    private GameObject objetoSostenido;
    private Rigidbody rbObjeto;
    private Camera camaraJugador;

    void Start()
    {
        camaraJugador = Camera.main;

        if (focusManager == null)
            focusManager = FindObjectOfType<EnfoqueEstacion>();
    }

    void Update()
    {
 
        if (Input.GetMouseButtonDown(0))
        {
            if (focusManager != null && focusManager.IsFocused)
            {

                if (objetoSostenido != null)
                {
                    HacerAnimacionCortar();
                }
            }
            else
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
        }

  
        if (objetoSostenido != null)
        {
            MoverObjetoSostenido();
        }
    }

    void IntentarAgarrar()
    {
        Ray rayo = camaraJugador.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit golpe;

        if (Physics.Raycast(rayo, out golpe, distanciaAlcance))
        {
            if (golpe.collider.CompareTag("Interactuable"))
            {
                objetoSostenido = golpe.collider.gameObject;
                rbObjeto = objetoSostenido.GetComponent<Rigidbody>();

                if (rbObjeto != null)
                {
                    rbObjeto.useGravity = false;
                    rbObjeto.isKinematic = true;
                }

                objetoSostenido.transform.SetParent(zonaSostener);

                if (objetoSostenido.GetComponent<ObjetoEstacion>() == null)
                {
                    objetoSostenido.AddComponent<ObjetoEstacion>();
                }

                EstacionesManager estacionCercana = null;
                float menorDistancia = 5f;
                EstacionesManager[] todas = FindObjectsOfType<EstacionesManager>();

                foreach (EstacionesManager est in todas)
                {
                    float dist = Vector3.Distance(objetoSostenido.transform.position, est.transform.position);
                    if (dist < menorDistancia)
                    {
                        menorDistancia = dist;
                        estacionCercana = est;
                    }
                }

                if (estacionCercana != null && focusManager != null)
                {
                    focusManager.FocusOnStation(estacionCercana);
                }
            }
        }
    }

    void MoverObjetoSostenido()
    {
        objetoSostenido.transform.position = zonaSostener.position;
        objetoSostenido.transform.rotation = zonaSostener.rotation;
    }

    void SoltarObjeto()
    {
        if (rbObjeto != null)
        {
            rbObjeto.useGravity = true;
            rbObjeto.isKinematic = false;
        }

        objetoSostenido.transform.SetParent(null);
        objetoSostenido = null;
    }

    void HacerAnimacionCortar()
    {
        Animator anim = objetoSostenido.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Cortar");
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(EfectoCorteProcedural());
        }
    }

    private IEnumerator EfectoCorteProcedural()
    {
        Vector3 posOriginalLocal = zonaSostener.localPosition;
        Vector3 posCorte = posOriginalLocal + new Vector3(0, -0.15f, 0.1f);

        float tiempo = 0f;
        while (tiempo < 0.08f)
        {
            zonaSostener.localPosition = Vector3.Lerp(posOriginalLocal, posCorte, tiempo / 0.08f);
            tiempo += Time.deltaTime;
            yield return null;
        }

        tiempo = 0f;
        while (tiempo < 0.08f)
        {
            zonaSostener.localPosition = Vector3.Lerp(posCorte, posOriginalLocal, tiempo / 0.08f);
            tiempo += Time.deltaTime;
            yield return null;
        }

        zonaSostener.localPosition = posOriginalLocal;
    }


    public void DevolverObjetoAEstacion()
    {
        if (objetoSostenido != null)
        {
            ObjetoEstacion infoObjeto = objetoSostenido.GetComponent<ObjetoEstacion>();
            if (infoObjeto != null)
            {
                infoObjeto.ResetearPosicionOriginal();
            }
            else
            {
                SoltarObjeto();
            }

            objetoSostenido = null;
        }
    }
}
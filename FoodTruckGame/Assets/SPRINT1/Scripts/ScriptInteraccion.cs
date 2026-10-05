using System.Collections;
using UnityEngine;

public class ScriptInteraccion : MonoBehaviour
{
    public float distanciaAlcance = 3f;
    public Transform zonaSostenerHerramienta;
    public Transform zonaSostenerIngrediente;
    public Transform zonaSostenerBowl;
    public Transform zonaSostenerCucharon;


    public Transform zonaSostenerCebollin;
    public Transform zonaSostenerDaikon;

    public GameObject puntoBlancoUI;
    public EnfoqueEstacion focusManager;

    private GameObject objetoSostenido;
    private Transform zonaActual;
    private Rigidbody rbObjeto;
    private Camera camaraJugador;

    void Start()
    {
        camaraJugador = Camera.main;

        if (focusManager == null)
            focusManager = FindObjectOfType<EnfoqueEstacion>();

        if (zonaSostenerIngrediente == null) zonaSostenerIngrediente = zonaSostenerHerramienta;
        if (zonaSostenerBowl == null) zonaSostenerBowl = zonaSostenerHerramienta;
        if (zonaSostenerCucharon == null) zonaSostenerCucharon = zonaSostenerHerramienta;
    }

    void Update()
    {
        if (focusManager != null && focusManager.IsFocused)
        {
            if (puntoBlancoUI != null && puntoBlancoUI.activeSelf)
                puntoBlancoUI.SetActive(false);
        }
        else
        {
            if (puntoBlancoUI != null && !puntoBlancoUI.activeSelf)
                puntoBlancoUI.SetActive(true);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (objetoSostenido == null)
            {
                IntentarAgarrar();
            }
            else
            {
                if (objetoSostenido.CompareTag("Cuchillo") && focusManager != null && focusManager.IsFocused)
                {
                    DevolverObjetoAEstacion();
                }
                else
                {
                    if (IntentarColocarEnZona() == false)
                    {
                        SoltarObjeto();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CambiarOQuitarEstacion();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (objetoSostenido != null)
            {
                if (objetoSostenido.CompareTag("Cuchillo") || objetoSostenido.name.ToLower().Contains("cuchillo"))
                {
                    HacerAnimacionCortar();
                }
                else if (objetoSostenido.GetComponentInParent<BowlContenedor>() != null || objetoSostenido.name.ToLower().Contains("bowl"))
                {
                    IntentarVaciarOlla();
                }
                else if (objetoSostenido.CompareTag("Cucharon") || objetoSostenido.name.ToLower().Contains("cucharon"))
                {
                    UsarCucharon();
                }
            }
        }

        if (objetoSostenido != null && zonaActual != null)
        {
            MoverObjetoSostenido();
        }
    }

    void IntentarAgarrar()
    {
        Ray rayo;

        if (focusManager != null && focusManager.IsFocused)
        {
            rayo = camaraJugador.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            rayo = camaraJugador.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        }

        RaycastHit golpe;

        if (Physics.Raycast(rayo, out golpe, distanciaAlcance))
        {
            GameObject objetoGolpeado = golpe.collider.gameObject;

            CajaIngrediente caja = objetoGolpeado.GetComponentInParent<CajaIngrediente>();
            if (caja != null)
            {
                GameObject verduraGenerada = caja.ExtraerIngrediente(zonaSostenerIngrediente);
                if (verduraGenerada != null)
                {
                    objetoSostenido = verduraGenerada;
                    rbObjeto = objetoSostenido.GetComponent<Rigidbody>();

                    if (rbObjeto != null)
                    {
                        rbObjeto.useGravity = false;
                        rbObjeto.isKinematic = true;
                    }

                    zonaActual = zonaSostenerIngrediente;
                    objetoSostenido.transform.SetParent(zonaActual);
                    objetoSostenido.transform.localPosition = Vector3.zero;
                    objetoSostenido.transform.localRotation = Quaternion.identity;

                    if (focusManager != null && !focusManager.IsFocused)
                    {
                        focusManager.EnfocarPrimerPunto();
                    }
                }
                return;
            }

            bool esCucharon = objetoGolpeado.CompareTag("Cucharon") || objetoGolpeado.name.ToLower().Contains("cucharon");

            if (objetoGolpeado.CompareTag("Interactuable") || objetoGolpeado.CompareTag("Cuchillo") || esCucharon ||
                objetoGolpeado.GetComponentInParent<Daikon_Cortar>() != null ||
                objetoGolpeado.GetComponentInParent<Cebollin_Cortar>() != null ||
                objetoGolpeado.GetComponentInParent<BowlContenedor>() != null) 
            {
                Daikon_Cortar daikon = objetoGolpeado.GetComponentInParent<Daikon_Cortar>();
                Cebollin_Cortar cebollin = objetoGolpeado.GetComponentInParent<Cebollin_Cortar>();
                BowlContenedor bowlScript = objetoGolpeado.GetComponentInParent<BowlContenedor>(); 

                if (daikon != null)
                {
                    objetoSostenido = daikon.gameObject;
                }
                else if (cebollin != null)
                {
                    objetoSostenido = cebollin.gameObject;
                }
                else if (bowlScript != null)
                {
                    objetoSostenido = bowlScript.gameObject;
                }
                else
                {
                    objetoSostenido = objetoGolpeado;
                }

                rbObjeto = objetoSostenido.GetComponent<Rigidbody>();

                if (rbObjeto != null)
                {
                    rbObjeto.useGravity = false;
                    rbObjeto.isKinematic = true;
                }


                if (objetoSostenido.GetComponentInChildren<BowlContenedor>() != null || objetoSostenido.name.ToLower().Contains("bowl"))
                {
                    zonaActual = zonaSostenerBowl;
                }
                else if (objetoSostenido.GetComponentInChildren<Cebollin_Cortar>() != null || objetoSostenido.name.ToLower().Contains("cebollin"))
                {
                    zonaActual = (zonaSostenerCebollin != null) ? zonaSostenerCebollin : zonaSostenerIngrediente;
                }
                else if (objetoSostenido.GetComponentInChildren<Daikon_Cortar>() != null || objetoSostenido.name.ToLower().Contains("daikon"))
                {
                    zonaActual = (zonaSostenerDaikon != null) ? zonaSostenerDaikon : zonaSostenerIngrediente;
                }
                else if (esCucharon)
                {
                    zonaActual = zonaSostenerCucharon;
                }
                else
                {
                    zonaActual = zonaSostenerHerramienta;
                }

                objetoSostenido.transform.SetParent(zonaActual);
                objetoSostenido.transform.localPosition = Vector3.zero;
                objetoSostenido.transform.localRotation = Quaternion.identity;

                if (objetoSostenido.CompareTag("Cuchillo") && objetoSostenido.GetComponent<ObjetoEstacion>() == null)
                {
                    objetoSostenido.AddComponent<ObjetoEstacion>();
                }

                if (focusManager != null && !focusManager.IsFocused)
                {
                    focusManager.EnfocarPrimerPunto();
                }
            }
        }
    }

    bool IntentarColocarEnZona()
    {
        Ray rayo;

        if (focusManager != null && focusManager.IsFocused)
        {
            rayo = camaraJugador.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            rayo = camaraJugador.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        }

        RaycastHit[] golpes = Physics.RaycastAll(rayo, distanciaAlcance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);

        foreach (RaycastHit golpe in golpes)
        {
            if (golpe.collider.transform.IsChildOf(objetoSostenido.transform) || golpe.collider.gameObject == objetoSostenido)
            {
                continue;
            }

            if (golpe.collider.CompareTag("ZonaReposoBowl"))
            {
                bool esBowl = (objetoSostenido.GetComponentInChildren<BowlContenedor>() != null || objetoSostenido.name.ToLower().Contains("bowl"));

                if (esBowl)
                {
                    objetoSostenido.transform.SetParent(null);
                    objetoSostenido.transform.position = golpe.collider.transform.position;
                    objetoSostenido.transform.rotation = golpe.collider.transform.rotation;
                    objetoSostenido.transform.localScale = Vector3.one;

                    if (rbObjeto != null)
                    {
                        rbObjeto.isKinematic = true;
                        rbObjeto.useGravity = false;
                    }

                    LimpiarMano();
                    return true;
                }
            }

            BowlContenedor bowlDestino = golpe.collider.GetComponentInParent<BowlContenedor>();
            if (bowlDestino != null)
            {
                MijoControl mijoSostenido = objetoSostenido.GetComponentInChildren<MijoControl>();
                Daikon_Cortar daikonSostenido = objetoSostenido.GetComponentInChildren<Daikon_Cortar>();
                Cebollin_Cortar cebollinSostenido = objetoSostenido.GetComponentInChildren<Cebollin_Cortar>();

                if (mijoSostenido != null)
                {
                    bowlDestino.AcomodarMijoEnBowl(objetoSostenido);
                    LimpiarMano();
                    return true;
                }
                else if (daikonSostenido != null && daikonSostenido.EstadoCorte == 2)
                {
                    bowlDestino.AcomodarEnBowl(objetoSostenido);
                    LimpiarMano();
                    return true;
                }
                else if (cebollinSostenido != null && cebollinSostenido.EstadoCorte == 2)
                {
                    bowlDestino.AcomodarEnBowl(objetoSostenido);
                    LimpiarMano();
                    return true;
                }
            }
        }

        return false;
    }

    void LimpiarMano()
    {
        objetoSostenido = null;
        rbObjeto = null;
        zonaActual = null;
    }

    void CambiarOQuitarEstacion()
    {
        if (focusManager == null) return;

        if (focusManager.IsFocused)
        {
            focusManager.CambiarASiguientePunto();
        }
        else
        {
            focusManager.EnfocarPrimerPunto();
        }
    }

    void MoverObjetoSostenido()
    {
        objetoSostenido.transform.position = zonaActual.position;
        objetoSostenido.transform.rotation = zonaActual.rotation;
    }

    void SoltarObjeto()
    {
        if (objetoSostenido == null) return;

        objetoSostenido.transform.SetParent(null);

        Collider[] colliders = objetoSostenido.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        if (rbObjeto != null)
        {
            rbObjeto.isKinematic = false;
            rbObjeto.useGravity = true;
        }

        LimpiarMano();
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
        if (zonaActual == null) yield break;

        Vector3 posOriginalLocal = zonaActual.localPosition;
        Vector3 posCorte = posOriginalLocal + new Vector3(0, -0.2f, 0.1f);

        float tiempo = 0f;
        while (tiempo < 0.08f)
        {
            if (zonaActual != null) zonaActual.localPosition = Vector3.Lerp(posOriginalLocal, posCorte, tiempo / 0.08f);
            tiempo += Time.deltaTime;
            yield return null;
        }

        tiempo = 0f;
        while (tiempo < 0.08f)
        {
            if (zonaActual != null) zonaActual.localPosition = Vector3.Lerp(posCorte, posOriginalLocal, tiempo / 0.08f);
            tiempo += Time.deltaTime;
            yield return null;
        }

        if (zonaActual != null) zonaActual.localPosition = posOriginalLocal;
    }

    public void DevolverObjetoAEstacion()
    {
        if (objetoSostenido != null)
        {
            ObjetoEstacion infoObjeto = objetoSostenido.GetComponent<ObjetoEstacion>();
            GameObject objDevolver = objetoSostenido;
            
            LimpiarMano();

            if (infoObjeto != null)
            {
                infoObjeto.ResetearPosicionOriginal();
            }
            else
            {
                objDevolver.transform.SetParent(null);
            }
        }
    }

    void IntentarVaciarOlla()
    {
        Ray rayo;
        if (focusManager != null && focusManager.IsFocused)
        {
            rayo = camaraJugador.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            rayo = camaraJugador.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        }

        RaycastHit[] golpes = Physics.RaycastAll(rayo, distanciaAlcance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

        foreach (RaycastHit golpe in golpes)
        {
            OllaCoccion olla = golpe.collider.GetComponentInParent<OllaCoccion>();

            if (olla != null)
            {
                StartCoroutine(EfectoInclinacionBowl());
                olla.RecibirVerduras(objetoSostenido);
                return;
            }
        }
    }

    private IEnumerator EfectoInclinacionBowl()
    {
        if (zonaActual == null) yield break;

        Quaternion rotOriginal = zonaActual.localRotation;
        Quaternion rotInclinada = rotOriginal * Quaternion.Euler(45f, 0, 0);

        float tiempo = 0f;
        while (tiempo < 0.2f)
        {
            if (zonaActual != null) zonaActual.localRotation = Quaternion.Lerp(rotOriginal, rotInclinada, tiempo / 0.2f);
            tiempo += Time.deltaTime;
            yield return null;
        }

        tiempo = 0f;
        while (tiempo < 0.2f)
        {
            if (zonaActual != null) zonaActual.localRotation = Quaternion.Lerp(rotInclinada, rotOriginal, tiempo / 0.2f);
            tiempo += Time.deltaTime;
            yield return null;
        }

        if (zonaActual != null) zonaActual.localRotation = rotOriginal;
    }

    void UsarCucharon()
    {
        Ray rayo;
        if (focusManager != null && focusManager.IsFocused)
        {
            rayo = camaraJugador.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            rayo = camaraJugador.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        }

        RaycastHit[] golpes = Physics.RaycastAll(rayo, distanciaAlcance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

        foreach (RaycastHit golpe in golpes)
        {
            if (golpe.collider.transform.IsChildOf(objetoSostenido.transform) || golpe.collider.gameObject == objetoSostenido)
            {
                continue;
            }

            OllaCoccion olla = golpe.collider.GetComponentInParent<OllaCoccion>();
            if (olla != null)
            {
                if (olla.SopaTerminada == true)
                {
                    GameObject porcion = olla.ExtraerSopa();

                    if (porcion != null)
                    {
                        Transform puntoSopa = objetoSostenido.transform.Find("PuntoSopa");

                        if (puntoSopa != null)
                        {
                            porcion.transform.SetParent(puntoSopa);
                        }
                        else
                        {
                            porcion.transform.SetParent(objetoSostenido.transform);
                        }

                        porcion.transform.localPosition = Vector3.zero;
                    }
                }
                return;
            }

            BowlContenedor bowl = golpe.collider.GetComponentInParent<BowlContenedor>();
            if (bowl == null && golpe.collider.name.ToLower().Contains("bowl"))
            {
                bowl = golpe.collider.gameObject.AddComponent<BowlContenedor>();
            }

            if (bowl != null || golpe.collider.name.ToLower().Contains("bowl"))
            {
                Transform transformBowl = (bowl != null) ? bowl.transform : golpe.collider.transform;

                foreach (Transform hijo in objetoSostenido.transform)
                {
                    if (hijo.name != "PuntoSopa")
                    {
                        hijo.SetParent(transformBowl);
                        hijo.localPosition = Vector3.zero;
                    }
                }

                Transform liquidoSopa = null;
                foreach (Transform hijo in transformBowl.GetComponentsInChildren<Transform>(true))
                {
                    if (hijo.name.ToLower().Replace(" ", "").Contains("liquidosopa") || hijo.name.ToLower() == "liquidosopa")
                    {
                        liquidoSopa = hijo;
                        break;
                    }
                }

                if (liquidoSopa != null)
                {
                    liquidoSopa.gameObject.SetActive(true);
                }
                return;
            }
        }
    }
}
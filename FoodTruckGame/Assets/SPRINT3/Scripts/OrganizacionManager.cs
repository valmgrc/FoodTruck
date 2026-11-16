using UnityEngine;
using System.Collections.Generic;

public class OrganizacionManager : MonoBehaviour
{
    [System.Serializable]
    public class CestaConfiguracion
    {
        public string nombreCesta = "Cesta";
        public GameObject objetoCesta;
        public ObjetoEmpacado[] objetosEnEstaCesta;

        [HideInInspector] public int indiceActual = 0;

        public bool TieneObjetosPendientes()
        {
            return objetosEnEstaCesta != null && indiceActual < objetosEnEstaCesta.Length;
        }
    }

    public CestaConfiguracion[] cestas;
    public Material materialBlancoInvalido;


    public TiempoLimite scriptTiempoPreparacion;
    public TutorialManager tutorialManager;
    public float distanciaRaycast = 6f;
    public LayerMask capasSuperficies = ~0;

    private ObjetoEmpacado objetoEnMano = null;
    private float rotacionYActual = 0f;
    private bool posicionValida = false;
    private Vector3 ultimaPosicionGrid;
    private Vector3 celdaBaseActual;

    private Dictionary<Vector3, List<ObjetoEmpacado>> mapaCeldas = new Dictionary<Vector3, List<ObjetoEmpacado>>();
    private bool faseActiva = true;

    void Start()
    {
        if (capasSuperficies.value == 0) capasSuperficies = ~0;

        if (scriptTiempoPreparacion != null)
            scriptTiempoPreparacion.enabled = false;

        foreach (var cesta in cestas)
        {
            if (cesta.objetosEnEstaCesta != null)
            {
                foreach (var obj in cesta.objetosEnEstaCesta)
                {
                    if (obj != null) obj.GuardarEnCesta();
                }
            }
        }

        if (tutorialManager != null)
        {
            tutorialManager.MostrarQuest(1, "Organización", "Acomoda los objetos que están dentro de la cesta");
        }
    }

    void Update()
    {
        if (!faseActiva) return;

        if (objetoEnMano == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                IntentarSacarDeAlgunaCesta();
            }
        }
        else
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll > 0f)
            {
                rotacionYActual -= 20f;
            }
            else if (scroll < 0f)
            {
                rotacionYActual += 20f;
            }

            ActualizarPosicionObjetoEnMano();

            if (Input.GetMouseButtonDown(0))
            {
                if (posicionValida)
                {
                    ColocarObjetoActual();
                }
            }
        }
    }

    void IntentarSacarDeAlgunaCesta()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaRaycast))
        {
            foreach (var cesta in cestas)
            {
                if (cesta.objetoCesta != null &&
                   (hit.collider.gameObject == cesta.objetoCesta || hit.collider.transform.IsChildOf(cesta.objetoCesta.transform)))
                {
                    if (cesta.TieneObjetosPendientes())
                    {
                        SacarSiguienteDeCesta(cesta);
                    }
                    else if (tutorialManager != null)
                    {
                        tutorialManager.MostrarQuest(1, "Organización", $"La {cesta.nombreCesta} ya está vacía.");
                    }
                    return;
                }
            }
        }
    }

    void SacarSiguienteDeCesta(CestaConfiguracion cesta)
    {
        objetoEnMano = cesta.objetosEnEstaCesta[cesta.indiceActual];
        cesta.indiceActual++;

        rotacionYActual = 0f;
        objetoEnMano.PrepararParaColocar();

        if (tutorialManager != null)
        {
            int totalesRestantes = ContarObjetosRestantesTotales();
            tutorialManager.MostrarQuest(1, "Organización", $"Acomoda: {objetoEnMano.nombreObjeto} (Clic para colocar, Rueda para rotar) - Faltan {totalesRestantes + 1}");
        }
    }

    Vector3 NormalizarCelda(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x * 100f) / 100f,
            Mathf.Round(pos.y * 100f) / 100f,
            Mathf.Round(pos.z * 100f) / 100f
        );
    }

    void ActualizarPosicionObjetoEnMano()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit[] impactos = Physics.RaycastAll(ray, distanciaRaycast, capasSuperficies, QueryTriggerInteraction.Ignore);
        System.Array.Sort(impactos, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in impactos)
        {
            if (hit.collider.GetComponentInParent<CharacterController>() != null) continue;

            SuperficieGrid superficie = hit.collider.GetComponentInParent<SuperficieGrid>();

            if (superficie != null)
            {
                Vector3 puntoSnap = superficie.ObtenerPuntoEnGrid(hit.point);
                Vector3 claveCelda = NormalizarCelda(puntoSnap);
                celdaBaseActual = claveCelda;
                Vector3 posicionFinalCalculada = puntoSnap;

                if (mapaCeldas.ContainsKey(claveCelda) && mapaCeldas[claveCelda].Count > 0)
                {
                    List<ObjetoEmpacado> objetosEnCelda = mapaCeldas[claveCelda];
                    if (objetoEnMano.esBowl && objetosEnCelda[0].esBowl)
                    {
                        posicionFinalCalculada = puntoSnap + Vector3.up * (objetoEnMano.alturaExtraApilado * objetosEnCelda.Count);
                        posicionValida = true;
                    }
                    else
                    {
                        posicionValida = false;
                    }
                }
                else
                {
                    posicionValida = true;
                }

                ultimaPosicionGrid = posicionFinalCalculada;

                objetoEnMano.ActualizarTransformConRotacion(posicionFinalCalculada, rotacionYActual);
                objetoEnMano.ActualizarVisualValidez(posicionValida, materialBlancoInvalido);
                return;
            }
            else
            {
                break;
            }
        }

        posicionValida = false;
        objetoEnMano.ActualizarTransformConRotacion(ray.GetPoint(1.5f), rotacionYActual);
        objetoEnMano.ActualizarVisualValidez(false, materialBlancoInvalido);
    }

    void ColocarObjetoActual()
    {
        objetoEnMano.ConfirmarColocacion(ultimaPosicionGrid, rotacionYActual);

        if (!mapaCeldas.ContainsKey(celdaBaseActual))
        {
            mapaCeldas[celdaBaseActual] = new List<ObjetoEmpacado>();
        }
        mapaCeldas[celdaBaseActual].Add(objetoEnMano);

        objetoEnMano = null;

        if (ContarObjetosRestantesTotales() == 0)
        {
            FinalizarFaseOrganizacion();
        }
        else if (tutorialManager != null)
        {
            tutorialManager.MostrarQuest(1, "Organización", "Haz clic en una cesta para sacar el siguiente objeto");
        }
    }

    int ContarObjetosRestantesTotales()
    {
        int total = 0;
        foreach (var cesta in cestas)
        {
            if (cesta.objetosEnEstaCesta != null)
            {
                total += (cesta.objetosEnEstaCesta.Length - cesta.indiceActual);
            }
        }
        return total;
    }

    void FinalizarFaseOrganizacion()
    {
        faseActiva = false;

        foreach (var cesta in cestas)
        {
            if (cesta.objetosEnEstaCesta != null)
            {
                foreach (var obj in cesta.objetosEnEstaCesta)
                {
                    if (obj != null) obj.ActivarParaCocina();
                }
            }

            if (cesta.objetoCesta != null)
                cesta.objetoCesta.SetActive(false);
        }

        if (scriptTiempoPreparacion != null)
            scriptTiempoPreparacion.enabled = true;

        if (tutorialManager != null)
        {
            tutorialManager.MostrarQuest(1, "Preparación", "Comienza a preparar tus platillos antes de que acabe el tiempo");
        }
    }
}
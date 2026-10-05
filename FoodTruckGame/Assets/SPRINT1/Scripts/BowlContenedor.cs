using UnityEngine;

public class BowlContenedor : MonoBehaviour
{
    public Transform puntoDeposito;

    private void OnTriggerEnter(Collider other)
    {
        Daikon_Cortar daikon = other.GetComponentInParent<Daikon_Cortar>();
        Cebollin_Cortar cebollin = other.GetComponentInParent<Cebollin_Cortar>();
        MijoControl mijo = other.GetComponentInParent<MijoControl>();

        if (daikon != null && daikon.EstadoCorte == 2)
        {
            AcomodarEnBowl(daikon.gameObject);
        }
        else if (cebollin != null && cebollin.EstadoCorte == 2)
        {
            AcomodarEnBowl(cebollin.gameObject);
        }
        else if (mijo != null) 
        {
            AcomodarMijoEnBowl(mijo.gameObject);
        }
    }

    public void AcomodarEnBowl(GameObject ingrediente)
    {
        Rigidbody rb = ingrediente.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Transform contenedorPadre = puntoDeposito != null ? puntoDeposito : transform;

        int cantidadActual = 0;
        foreach (Transform hijo in contenedorPadre)
        {
            cantidadActual++;
        }

        float alturaApilada = cantidadActual * 0.06f; 

        float randomX = Random.Range(-0.02f, 0.02f);
        float randomZ = Random.Range(-0.02f, 0.02f);

        ingrediente.transform.SetParent(contenedorPadre);
        ingrediente.transform.localPosition = new Vector3(randomX, alturaApilada, randomZ);
        ingrediente.transform.localRotation = Quaternion.identity;

        Daikon_Cortar daikonScript = ingrediente.GetComponentInChildren<Daikon_Cortar>();
        if (daikonScript != null && daikonScript.daikonPicado != null)
        {
            daikonScript.daikonPicado.transform.localPosition = Vector3.zero;
            daikonScript.daikonPicado.transform.localRotation = Quaternion.identity;
        }

        Cebollin_Cortar cebollinScript = ingrediente.GetComponentInChildren<Cebollin_Cortar>();
        if (cebollinScript != null && cebollinScript.cebollinPicado != null)
        {
            cebollinScript.cebollinPicado.transform.localPosition = Vector3.zero;
            cebollinScript.cebollinPicado.transform.localRotation = Quaternion.identity;
        }

        ingrediente.layer = 2;
        foreach (Transform hijo in ingrediente.GetComponentsInChildren<Transform>())
        {
            hijo.gameObject.layer = 2;
        }
    }

    public void AcomodarMijoEnBowl(GameObject mijoObj)
    {
        Rigidbody rb = mijoObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Transform contenedorPadre = puntoDeposito != null ? puntoDeposito : transform;

        mijoObj.transform.SetParent(contenedorPadre);
        mijoObj.transform.localPosition = Vector3.zero;
        mijoObj.transform.localRotation = Quaternion.identity;

        MijoControl mijoScript = mijoObj.GetComponentInChildren<MijoControl>();
        if (mijoScript != null)
        {
            mijoScript.DepositarEnBowl();
        }
        else
        {

        }

        Collider[] cols = mijoObj.GetComponentsInChildren<Collider>(true);
        foreach (Collider c in cols)
        {
            c.enabled = false;
        }

        mijoObj.layer = 2;
        foreach (Transform hijo in mijoObj.GetComponentsInChildren<Transform>(true))
        {
            hijo.gameObject.layer = 2;
        }

    }
}
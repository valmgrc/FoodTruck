using UnityEngine;

public class MijoControl : MonoBehaviour
{
 
    public GameObject objetoEsferaMijo;

    public GameObject objetoPlanoMijo;

    public float alturaOffsetPlano = 0.05f;

    public float zOffsetPlano = 0f; 

    void Start()
    {
        if (objetoEsferaMijo != null) objetoEsferaMijo.SetActive(true);
        if (objetoPlanoMijo != null) objetoPlanoMijo.SetActive(false);
    }


    public void DepositarEnBowl()
    {
        if (objetoEsferaMijo != null)
        {
            objetoEsferaMijo.SetActive(false);
            Destroy(objetoEsferaMijo);
        }

        if (objetoPlanoMijo != null)
        {
            objetoPlanoMijo.SetActive(true);

            objetoPlanoMijo.transform.localPosition = new Vector3(0f, alturaOffsetPlano, zOffsetPlano);
            objetoPlanoMijo.transform.localRotation = Quaternion.identity;
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

    }
}
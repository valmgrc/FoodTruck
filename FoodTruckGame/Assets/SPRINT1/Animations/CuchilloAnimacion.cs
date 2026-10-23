using UnityEngine;

public class CuchilloAnimacion : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0) && SistemaCorteObjeto.cuchilloEnMano)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                
                if (hit.transform.TryGetComponent<SistemaCorteObjeto>(out SistemaCorteObjeto sistemaCorte))
                {
             
                    anim.SetTrigger("Corte");
                }
            }
        }
    }
}
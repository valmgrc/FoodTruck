using UnityEngine;

public class PlayerPickDrop : MonoBehaviour
{

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    //punto para el cuchillo
    [SerializeField] private Transform objectGrabKnifeTransform;
    [SerializeField] private LayerMask pickUpLayerMask;

    private ObjectSelected objectSelected;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectSelected == null)
            { //no objeto en mano, agarrar
                float pickupDistance = 6f;
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupDistance, pickUpLayerMask))
                {

                    if (raycastHit.transform.TryGetComponent(out objectSelected))
                    {
                        if (raycastHit.transform.CompareTag("Cuchillo"))
                        {
                            //si es el cuchillo
                            objectSelected.Grab(objectGrabKnifeTransform);

                            Collider[] collidersCuchillo = objectSelected.GetComponentsInChildren<Collider>();
                            foreach (Collider col in collidersCuchillo)
                            {
                                col.enabled = false;
                            }

                            SistemaCorteObjeto.cuchilloEnMano = true;
                        }
                        else
                        {
                            //si es otro objeto
                            objectSelected.Grab(objectGrabPointTransform);
                        }


                    }
                }

            }
            else
            {
                //objeto en mano, soltar
                objectSelected.Drop();
                SistemaCorteObjeto.cuchilloEnMano = false;

                if (objectSelected != null)
                {
                    // Buscamos y reactivamos los colliders del objeto que teníamos en la mano
                    Collider[] collidersObjeto = objectSelected.GetComponentsInChildren<Collider>();
                    foreach (Collider col in collidersObjeto)
                    {
                        col.enabled = true; // ¡Vuelven a encenderse!
                    }
                    objectSelected = null;
                }
            }
        }
    }
}

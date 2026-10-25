using UnityEngine;

public class PlayerPickDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;

    [SerializeField] private Transform objectGrabKnifeTransform;
    [SerializeField] private Transform objectGrabCucharonTransform;

    [SerializeField] private LayerMask pickUpLayerMask;

    private ObjectSelected objectSelected;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectSelected == null)
            {
                float pickupDistance = 6f;
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupDistance, pickUpLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent(out objectSelected))
                    {
                        if (raycastHit.transform.CompareTag("Cuchillo"))
                        {
                            objectSelected.Grab(objectGrabKnifeTransform);

                            Collider[] collidersCuchillo = objectSelected.GetComponentsInChildren<Collider>();
                            foreach (Collider col in collidersCuchillo)
                            {
                                col.enabled = false;
                            }

                            SistemaCorteObjeto.cuchilloEnMano = true;
                        }
                        else if (raycastHit.transform.CompareTag("Cucharon")) 
                        {
                            objectSelected.Grab(objectGrabCucharonTransform);

                            Collider[] collidersCucharon = objectSelected.GetComponentsInChildren<Collider>();
                            foreach (Collider col in collidersCucharon)
                            {
                                col.enabled = false;
                            }

                            CucharonSystem.cucharonEnMano = true; 
                        }
                        else
                        {
                            objectSelected.Grab(objectGrabPointTransform);
                        }
                    }
                }
            }
            else
            {
                objectSelected.Drop();

                SistemaCorteObjeto.cuchilloEnMano = false;
                CucharonSystem.cucharonEnMano = false;

                if (objectSelected != null)
                {
                    Collider[] collidersObjeto = objectSelected.GetComponentsInChildren<Collider>();
                    foreach (Collider col in collidersObjeto)
                    {
                        col.enabled = true;
                    }
                    objectSelected = null;
                }
            }
        }
    }
}
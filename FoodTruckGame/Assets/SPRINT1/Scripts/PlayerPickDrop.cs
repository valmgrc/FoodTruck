using UnityEngine;

public class PlayerPickDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;

    [SerializeField] private Transform objectGrabKnifeTransform;
    [SerializeField] private Transform objectGrabCucharonTransform;
    [SerializeField] private Transform objectGrabTelaTransform;


    [SerializeField] private Transform objectGrabBowlLlenoTransform; 
    [SerializeField] private Transform objectGrabCajaLlenaTransform; 

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
                        else if (raycastHit.transform.CompareTag("Tela") || raycastHit.transform.name.Contains("Tela"))
                        {
                            if (objectGrabTelaTransform != null)
                            {
                                objectSelected.Grab(objectGrabTelaTransform);
                            }
                            else
                            {
                                objectSelected.Grab(objectGrabPointTransform);
                            }
                        }
                        else if (raycastHit.transform.TryGetComponent(out ControladorBowl bowl) && bowl.estaLleno)
                        {
                            if (objectGrabBowlLlenoTransform != null)
                            {
                                objectSelected.Grab(objectGrabBowlLlenoTransform);
                            }
                            else
                            {
                                objectSelected.Grab(objectGrabPointTransform);
                            }
                        }
                        else if (raycastHit.transform.TryGetComponent(out ControladorCajaLlevar caja) && caja.estaLleno)
                        {
                            if (objectGrabCajaLlenaTransform != null)
                            {
                                objectSelected.Grab(objectGrabCajaLlenaTransform);
                            }
                            else
                            {
                                objectSelected.Grab(objectGrabPointTransform);
                            }
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
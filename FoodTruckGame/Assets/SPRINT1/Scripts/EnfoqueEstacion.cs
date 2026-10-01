using System.Collections;
using UnityEngine;

public class EnfoqueEstacion : MonoBehaviour
{

    public Camera mainCamera;

    public MonoBehaviour starterAssetsInputsScript;
 
    public MonoBehaviour firstPersonControllerScript;

 
    public float transitionSpeed = 7f;


    public float sensibilidadMouse = 2f;
    public float maxGiroHorizontal = 25f;
    public float maxGiroVertical = 15f; 

    private EstacionesManager currentStation;
    private bool isFocused = false;
    public bool IsFocused => isFocused;

    private Transform originalCameraParent;
    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;

    private Behaviour cinemachineBrain;

 
    private float rotacionX = 0f;
    private float rotacionY = 0f;
    private Quaternion rotacionBaseEstacion;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
        {
            originalCameraParent = mainCamera.transform.parent;
            originalLocalPos = mainCamera.transform.localPosition;
            originalLocalRot = mainCamera.transform.localRotation;

            cinemachineBrain = mainCamera.GetComponent("CinemachineBrain") as Behaviour;
        }
    }

    void Update()
    {
        if (!isFocused) return;


        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
        {
            ExitStation();
            return;
        }

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
            float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

            rotacionY += mouseX;
            rotacionX -= mouseY;


            rotacionY = Mathf.Clamp(rotacionY, -maxGiroHorizontal, maxGiroHorizontal);
            rotacionX = Mathf.Clamp(rotacionX, -maxGiroVertical, maxGiroVertical);

            Quaternion rotacionObjetivo = rotacionBaseEstacion * Quaternion.Euler(rotacionX, rotacionY, 0f);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, rotacionObjetivo, Time.deltaTime * 10f);
        }
    }

    public void FocusOnStation(EstacionesManager station)
    {
        if (isFocused || station == null || station.cameraFocusPoint == null) return;

        currentStation = station;
        isFocused = true;


        rotacionX = 0f;
        rotacionY = 0f;

        if (firstPersonControllerScript != null)
            firstPersonControllerScript.enabled = false;

        if (starterAssetsInputsScript != null)
            starterAssetsInputsScript.enabled = false;

        if (cinemachineBrain != null)
            cinemachineBrain.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        mainCamera.transform.SetParent(null);

        rotacionBaseEstacion = station.cameraFocusPoint.rotation;

        StopAllCoroutines();
        StartCoroutine(MoveCameraToTarget(station.cameraFocusPoint.position, station.cameraFocusPoint.rotation));
    }

    public void ExitStation()
    {
        if (!isFocused) return;

  
        ScriptInteraccion interaccion = FindObjectOfType<ScriptInteraccion>();
        if (interaccion != null)
        {
            interaccion.DevolverObjetoAEstacion();
        }

        isFocused = false;
        currentStation = null;

        StopAllCoroutines();
        StartCoroutine(ReturnCameraToPlayer());
    }

    private IEnumerator MoveCameraToTarget(Vector3 targetPos, Quaternion targetRot)
    {
        while (Vector3.Distance(mainCamera.transform.position, targetPos) > 0.01f ||
               Quaternion.Angle(mainCamera.transform.rotation, targetRot) > 0.1f)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, Time.deltaTime * transitionSpeed);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, targetRot, Time.deltaTime * transitionSpeed);
            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;
    }

    private IEnumerator ReturnCameraToPlayer()
    {
        Vector3 targetPos = originalCameraParent.TransformPoint(originalLocalPos);
        Quaternion targetRot = originalCameraParent.rotation * originalLocalRot;

        while (Vector3.Distance(mainCamera.transform.position, targetPos) > 0.05f)
        {
            targetPos = originalCameraParent.TransformPoint(originalLocalPos);
            targetRot = originalCameraParent.rotation * originalLocalRot;

            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, Time.deltaTime * transitionSpeed);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, targetRot, Time.deltaTime * transitionSpeed);
            yield return null;
        }

        mainCamera.transform.SetParent(originalCameraParent);
        mainCamera.transform.localPosition = originalLocalPos;
        mainCamera.transform.localRotation = originalLocalRot;

        if (cinemachineBrain != null)
            cinemachineBrain.enabled = true;

        if (firstPersonControllerScript != null)
            firstPersonControllerScript.enabled = true;

        if (starterAssetsInputsScript != null)
            starterAssetsInputsScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
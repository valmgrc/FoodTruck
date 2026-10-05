using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnfoqueEstacion : MonoBehaviour
{
    public Camera mainCamera;

    
    public List<Transform> puntosDeEnfoque = new List<Transform>();
    private int indiceEstacionActual = 0;

    public MonoBehaviour starterAssetsInputsScript;
    public MonoBehaviour firstPersonControllerScript;
    public float transitionSpeed = 7f;
    public float sensibilidadMouse = 2f;
    public float maxGiroHorizontal = 25f;
    public float maxGiroVertical = 15f;

    private bool isFocused = false;
    public bool IsFocused
    {
        get { return isFocused; }
        set { isFocused = value; }
    }

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

        if (Input.GetKeyDown(KeyCode.E))
        {
            CambiarASiguientePunto();
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

    public void EnfocarPrimerPunto()
    {
        if (puntosDeEnfoque.Count == 0) return;

        indiceEstacionActual = 0;
        MoverAlPuntoActual();
    }

    public void CambiarASiguientePunto()
    {
        if (puntosDeEnfoque.Count <= 1) return;

        indiceEstacionActual = (indiceEstacionActual + 1) % puntosDeEnfoque.Count;
        MoverAlPuntoActual();
    }

    private void MoverAlPuntoActual()
    {
        Transform puntoObjetivo = puntosDeEnfoque[indiceEstacionActual];
        if (puntoObjetivo == null) return;

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

        rotacionBaseEstacion = puntoObjetivo.rotation;

        StopAllCoroutines();
        StartCoroutine(MoveCameraToTarget(puntoObjetivo.position, puntoObjetivo.rotation));
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

        StopAllCoroutines();
        StartCoroutine(ReturnCameraToPlayer());
    }

    public void Unfocus()
    {
        ExitStation();
    }

    private IEnumerator MoveCameraToTarget(Vector3 targetPos, Quaternion targetRot)
    {
        mainCamera.transform.SetParent(null, true);

        float t = 0f;
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * (transitionSpeed / 2f);
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;
    }

    private IEnumerator ReturnCameraToPlayer()
    {
        Vector3 targetPos = originalCameraParent.TransformPoint(originalLocalPos);
        Quaternion targetRot = originalCameraParent.rotation * originalLocalRot;

        float t = 0f;
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            targetPos = originalCameraParent.TransformPoint(originalLocalPos);
            targetRot = originalCameraParent.rotation * originalLocalRot;

            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
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
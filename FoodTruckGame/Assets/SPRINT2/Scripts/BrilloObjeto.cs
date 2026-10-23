using UnityEngine;

public class BrilloObjeto : MonoBehaviour
{
    public float velocidadParpadeo = 1f;
    public float escalaMinima = 0.1f;
    public float escalaMaxima = 1.2f;

    private Vector3 escalaOriginal;

    void Start()
    {
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        float escalaDinamica = Mathf.Lerp(escalaMinima, escalaMaxima, (Mathf.Sin(Time.time * velocidadParpadeo) + 1f) / 2f);
        transform.localScale = escalaOriginal * escalaDinamica;

        transform.LookAt(Camera.main.transform);
    }
}
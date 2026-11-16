using UnityEngine;

public class SuperficieGrid : MonoBehaviour
{
    public float tamanoCelda = 0.25f;


    public Vector3 ObtenerPuntoEnGrid(Vector3 puntoImpactoMundial)
    {
        Vector3 puntoLocal = transform.InverseTransformPoint(puntoImpactoMundial);

        puntoLocal.x = Mathf.Round(puntoLocal.x / tamanoCelda) * tamanoCelda;
        puntoLocal.z = Mathf.Round(puntoLocal.z / tamanoCelda) * tamanoCelda;

        return transform.TransformPoint(puntoLocal);
    }

    private void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        Gizmos.color = new Color(0f, 1f, 0.5f, 0.35f);
        Bounds b = col.bounds;
        float y = b.max.y + 0.005f;

        for (float x = b.min.x; x <= b.max.x; x += tamanoCelda)
        {
            Gizmos.DrawLine(new Vector3(x, y, b.min.z), new Vector3(x, y, b.max.z));
        }
        for (float z = b.min.z; z <= b.max.z; z += tamanoCelda)
        {
            Gizmos.DrawLine(new Vector3(b.min.x, y, z), new Vector3(b.max.x, y, z));
        }
    }
}
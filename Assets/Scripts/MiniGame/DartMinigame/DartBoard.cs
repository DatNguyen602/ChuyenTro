using UnityEngine;

public class DartBoard : MonoBehaviour
{
    [Header("Ring Radii ")]
    [SerializeField] private float innerBullRadius = 0.16f;
    [SerializeField] private float outerBullRadius = 0.28f;
    [SerializeField] private float tripleRingInner = 1.02f;
    [SerializeField] private float tripleRingOuter = 1.12f;
    [SerializeField] private float doubleRingInner = 1.63f;
    [SerializeField] private float doubleRingOuter = 1.78f;
    [SerializeField] private float boardRadius = 2.26f;    
    public float BoardRadius=>boardRadius;

    [Header("Sector")]
    [Range(3, 60)]
    [SerializeField] private int sectorCount = 20;

    readonly int[] sectorScores = {
        1, 18, 4, 13, 6, 10, 15, 2, 17,
        3, 19, 7, 16, 8, 11, 14, 9, 12, 5, 20
    };


    

  
   
    public int GetScore(Vector2 worldPoint)
    {
        Vector2 local = transform.InverseTransformPoint(worldPoint);
        float r = local.magnitude;

        if (r <= innerBullRadius) return 50;
        if (r <= outerBullRadius) return 25;

        int mult = 0;
        if (r >= tripleRingInner && r <= tripleRingOuter) mult = 3;
        else if (r >= doubleRingInner && r <= doubleRingOuter) mult = 2;
        else if (r <= boardRadius) mult = 1;
        else return 0;

        float angleDeg = 90f - Mathf.Atan2(local.y, local.x) * Mathf.Rad2Deg ;
        if (angleDeg < 0) angleDeg += 360f;

        float sectorAngle = 360f / sectorCount;
        int idx = Mathf.FloorToInt(angleDeg / sectorAngle) % sectorCount;
        int baseScore = sectorScores[idx];

        Debug.Log(idx);

        return baseScore * mult;
    }

    

    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Vector3.zero, innerBullRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, outerBullRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, tripleRingInner);
        Gizmos.DrawWireSphere(Vector3.zero, tripleRingOuter);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(Vector3.zero, doubleRingInner);
        Gizmos.DrawWireSphere(Vector3.zero, doubleRingOuter);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(Vector3.zero, boardRadius);

        Gizmos.color = Color.white;
        float step = 360f / sectorCount;
        for (int i = 0; i < sectorCount; i++)
        {
            float a = i * step + 90f;
            float rad = a * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
            Gizmos.DrawLine(Vector3.zero, dir * boardRadius);
        }
    }
}

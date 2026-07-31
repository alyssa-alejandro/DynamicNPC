using UnityEngine;

public class ScoutPOIAction : UtilityAction
{
    [Header("Vision Settings")]
    [SerializeField] private float m_FieldOfViewAngle = 120.0f; // Total FOV arc (e.g., 60 deg left, 60 deg right)
    [SerializeField] private float m_MaxSearchRadius = 30f;

    private Transform m_CurrentTargetPOI;

    public override float EvaluateUtility(PlayerProfile profile, Transform agent, Transform player)
    {
        // Pass 'agent' so we can check agent.forward for FOV!
        m_CurrentTargetPOI = FindNearestPOI(agent.position, agent);
        
        if (m_CurrentTargetPOI == null) return 0.0f;

        float distance = Vector3.Distance(agent.position, m_CurrentTargetPOI.position);
        if (distance > m_MaxSearchRadius) return 0.0f;

        float distanceScore = Mathf.Clamp01(1.0f - (distance / m_MaxSearchRadius));
        float utility = distanceScore * profile.Type5_Investigator;

        return Mathf.Clamp01(utility);
    }

    public override void Execute(Transform agent, Transform player)
    {
        // Navigation / Scouting logic here
        Debug.Log("[Utility AI] Executing: Scouting POI");
    }

    private Transform FindNearestPOI(Vector3 origin, Transform agent)
    {
        Collider[] colliders = Physics.OverlapSphere(origin, m_MaxSearchRadius);
        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (Collider col in colliders)
        {
            // 1. Tag Filter
            if (!col.CompareTag("PointOfInterest")) continue;

            // 2. FOV Check
            Vector3 directionToTarget = (col.transform.position - origin).normalized;
            float angleToTarget = Vector3.Angle(agent.forward, directionToTarget);

            // Ignore if object is outside the forward cone of vision (half-angle check)
            if (angleToTarget > m_FieldOfViewAngle * 0.5f) continue;

            // 3. Proximity Check
            float dist = Vector3.Distance(origin, col.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = col.transform;
            }
        }

        return closest;
    }
}
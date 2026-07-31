using UnityEngine;

public class ScoutPOIAction : UtilityAction
{
    [Header("Vision Settings")]
    [SerializeField] private float m_FieldOfViewAngle = 120.0f; // Total FOV arc (e.g., 60 deg left, 60 deg right)
    [SerializeField] private float m_MaxSearchRadius = 15f;

    [Tooltip("Layers that block line of sight (e.g., Default, Environment, Wall). Do NOT include the POI layer.")]
    [SerializeField] private LayerMask m_ObstacleMask = -1; // Default to Everything (-1)

    [Header("Navigation Settings")]
    [SerializeField] private float m_MovementSpeed = 4.0f;
    [SerializeField] private float m_ArrivalDistance = 1.5f;

    // Private runtime state variables
    private Transform m_CurrentTargetPOI;
    private UnityEngine.AI.NavMeshAgent m_NavAgent;


    public override float EvaluateUtility(PlayerProfile profile, Transform agent, Transform player)
    {
        // Pass 'agent' so we can check agent.forward for FOV!
        m_CurrentTargetPOI = FindNearestPOI(agent.position, agent);
        
        if (m_CurrentTargetPOI == null) return 0.0f;

        float distance = Vector3.Distance(agent.position, m_CurrentTargetPOI.position);
        // Debug.Log($"[Utility AI] ScoutPOIAction: Found POI at distance {distance:F2} units.");
        if (distance > m_MaxSearchRadius) return 0.0f;

        float distanceScore = Mathf.Clamp01(1.0f - (distance / m_MaxSearchRadius));
        float utility = distanceScore * profile.Type5_Investigator;

        // Debug.Log($"[Utility AI] Evaluated ScoutPOIAction: Distance Score = {distanceScore:F2}, Type5_Investigator = {profile.Type5_Investigator:F2}, Final Utility = {utility:F2}");
        return Mathf.Clamp01(utility);
    }

    public override void Execute(Transform agent, Transform player)
    {
        // Debug.Log("[ScoutPOIAction] Executing: Scouting Point of Interest");
        // Navigation / Scouting logic here

        if (m_CurrentTargetPOI == null) return;

        // 1. Fetch NavMeshAgent
        if (m_NavAgent == null) m_NavAgent = agent.GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (m_NavAgent != null && m_NavAgent.isOnNavMesh)
        {
            m_NavAgent.speed = m_MovementSpeed;
            m_NavAgent.stoppingDistance = m_ArrivalDistance;
            m_NavAgent.SetDestination(m_CurrentTargetPOI.position);

            // 2. Check if arrived at POI
            float distanceToPOI = Vector3.Distance(agent.position, m_CurrentTargetPOI.position);
            
            if (distanceToPOI <= (m_ArrivalDistance + 0.5f))
            {
                // 3. Mark POI as Explored
                if (m_CurrentTargetPOI.TryGetComponent<PointOfInterest>(out var poi))
                {
                    poi.MarkAsExplored();
                    Debug.Log($"[ScoutPOIAction] Successfully arrived and marked {m_CurrentTargetPOI.name} as Explored.");
                }

                // Reset destination and target reference
                m_CurrentTargetPOI = null;
                m_NavAgent.ResetPath();
            }
        }
    }

    private Transform FindNearestPOI(Vector3 origin, Transform agent)
    {
        Collider[] colliders = Physics.OverlapSphere(origin, m_MaxSearchRadius);
        Transform closest = null;
        float minDistance = float.MaxValue;
        Debug.Log($"[ScoutPOIAction] Colliders found: {colliders.Length}. Searching for nearest unexplored POI within FOV and line of sight.");
        // Offset origin slightly upward to eye level (1.5m) so raycasts don't hit the ground
        Vector3 eyePosition = origin + Vector3.up * 1.5f;

        foreach (Collider col in colliders)
        {
            // 1. Tag Filter
            if (!col.CompareTag("PointOfInterest")) continue;

            if (col.TryGetComponent<PointOfInterest>(out var poiComponent))
            {
                if (poiComponent.isExplored) continue; 
            }

            GameObject targetObj = col.gameObject;
            Debug.Log($"[ScoutPOIAction] Evaluating POI: {targetObj.name} at position {col.transform.position}");
            Vector3 targetPos = col.bounds.center; // Target center of POI collider
            Vector3 direction = targetPos - eyePosition;

            // 2. FOV Check
            float angleToTarget = Vector3.Angle(agent.forward, direction.normalized);
            if (angleToTarget > m_FieldOfViewAngle * 0.5f) continue;

            // Draw ray in Scene view for debugging line of sight
            Debug.DrawLine(eyePosition, targetPos, Color.red, 0.2f);

            // 3. Line of Sight Check using Raycast + ObstacleMask
            if (Physics.Raycast(eyePosition, direction.normalized, out RaycastHit hit, direction.magnitude, m_ObstacleMask))
            {
                // If raycast hits something that isn't the target POI or its child, LOS is blocked
                if (hit.transform.gameObject != targetObj && !hit.transform.IsChildOf(targetObj.transform))
                {
                    // Line of sight blocked by wall/obstacle!
                    continue; 
                }
            }

            // 4. Proximity Check
            float dist = Vector3.Distance(origin, col.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = col.transform;
            }
        }

        return closest;
    }

    private void OnDrawGizmosSelected()
{
    // 1. Draw Search Radius Sphere
    Gizmos.color = new Color(0f, 1f, 0.5f, 0.2f); // Semi-transparent cyan/green
    Gizmos.DrawSphere(transform.position, m_MaxSearchRadius);

    Gizmos.color = new Color(0f, 1f, 0.5f, 0.8f);
    Gizmos.DrawWireSphere(transform.position, m_MaxSearchRadius);

    // 2. Draw Eye Position Marker
    Vector3 eyePosition = transform.position + Vector3.up * 1.5f;
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(eyePosition, 0.15f);

    // 3. Draw FOV Cone Bounds
    float halfFOV = m_FieldOfViewAngle * 0.5f;
    
    // Calculate FOV boundary vectors
    Vector3 leftRayDirection = Quaternion.Euler(0, -halfFOV, 0) * transform.forward;
    Vector3 rightRayDirection = Quaternion.Euler(0, halfFOV, 0) * transform.forward;

    Gizmos.color = Color.yellow;
    Gizmos.DrawRay(eyePosition, leftRayDirection * m_MaxSearchRadius);
    Gizmos.DrawRay(eyePosition, rightRayDirection * m_MaxSearchRadius);

    // Draw Forward Line
    Gizmos.color = Color.blue;
    Gizmos.DrawRay(eyePosition, transform.forward * m_MaxSearchRadius);
}
}
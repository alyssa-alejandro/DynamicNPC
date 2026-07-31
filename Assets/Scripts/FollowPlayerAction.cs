using UnityEngine;
using UnityEngine.AI;

public class FollowPlayerAction : UtilityAction
{
    [SerializeField] private float m_BaseUtility = 0.1f; 
    [SerializeField] private float m_FollowDistanceThreshold = 2.0f;
    [SerializeField] private float m_NormalSpeed = 3.5f;
    
    // Add a dedicated stopping distance so the agent doesn't overlap
    [SerializeField] private float m_StoppingDistance = 1.5f; 

    private NavMeshAgent m_NavAgent;

    private void Awake()
    {
        ActionName = "Follow Player";
    }

    public override float EvaluateUtility(PlayerProfile profile, Transform agent, Transform player)
    {
        if (player == null) return 0.0f;

        float distance = Vector3.Distance(agent.position, player.position);
        
        if (distance > m_FollowDistanceThreshold)
        {
            return Mathf.Clamp01(m_BaseUtility + (distance / 200.0f));
        }

        return m_BaseUtility;
    }

    public override void Execute(Transform agent, Transform player)
    {
        // Debug.Log("[FollowPlayerAction] Executing: Following Player");
        if (m_NavAgent == null) m_NavAgent = agent.GetComponent<NavMeshAgent>();

        if (m_NavAgent != null && m_NavAgent.isOnNavMesh)
        {
            m_NavAgent.speed = m_NormalSpeed;

            // ⚡ Set stopping distance to keep the companion at a comfortable radius
            m_NavAgent.stoppingDistance = m_StoppingDistance; 

            m_NavAgent.SetDestination(player.position);
        }
    }
}
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayerAction : UtilityAction
{
    [SerializeField] private float m_BaseUtility = 0.1f; // Low constant base score so other actions win if valid
    [SerializeField] private float m_FollowDistanceThreshold = 3.0f;
    [SerializeField] private float m_NormalSpeed = 3.5f;

    private NavMeshAgent m_NavAgent;

    private void Awake()
    {
        ActionName = "Follow Player";
    }

    public override float EvaluateUtility(PlayerProfile profile, Transform agent, Transform player)
    {
        if (player == null) return 0.0f;

        float distance = Vector3.Distance(agent.position, player.position);
        
        // Increases utility slightly if player gets further away to stay close
        if (distance > m_FollowDistanceThreshold)
        {
            return Mathf.Clamp01(m_BaseUtility + (distance / 20.0f));
        }

        return m_BaseUtility;
    }

    public override void Execute(Transform agent, Transform player)
    {
        if (m_NavAgent == null) m_NavAgent = agent.GetComponent<NavMeshAgent>();

        if (m_NavAgent != null && m_NavAgent.isOnNavMesh)
        {
            m_NavAgent.speed = m_NormalSpeed;
            m_NavAgent.SetDestination(player.position);
        }
    }
}
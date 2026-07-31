using UnityEngine;

public class DrawAggroAction : UtilityAction
{
    [SerializeField] private float m_CombatRadius = 15f;
    [SerializeField] private LayerMask m_EnemyLayer;

    public override float EvaluateUtility(PlayerProfile profile, Transform agent, Transform player)
    {
        // 1. Environmental Signal: Are hostiles present?
        bool enemiesNearby = Physics.CheckSphere(agent.position, m_CombatRadius, m_EnemyLayer);
        if (!enemiesNearby) return 0.0f;

        // 2. Continuous Weighting: Scaled directly by Type 8 / Aggression EMA
        float utility = profile.Type8_Challenger;

        return Mathf.Clamp01(utility);
    }

    public override void Execute(Transform agent, Transform player)
    {
        Debug.Log("[Utility AI] Executing: Drawing Enemy Aggro");
    }
}
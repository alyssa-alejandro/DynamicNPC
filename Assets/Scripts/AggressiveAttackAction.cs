using UnityEngine;
using UnityEngine.AI;

public class AggressiveAttackAction : UtilityAction
{
    [Header("Combat Settings")]
    [SerializeField] private float m_CombatRadius = 15f;
    [SerializeField] private float m_AttackRange = 2.5f;
    [SerializeField] private float m_Damage = 20.0f;
    [SerializeField] private float m_AttackCooldown = 0.8f;
    [SerializeField] private LayerMask m_EnemyLayer;

    [Header("Navigation Settings")]
    [SerializeField] private float m_MovementSpeed = 6.5f; // Slightly faster to keep up with aggressive play

    [Header("Audio Settings")]
    [SerializeField] private AudioClip m_AttackAudioClip;
    [Range(0f, 1f)] [SerializeField] private float m_AttackAudioVolume = 0.8f;

    // Internal state
    private Transform m_CurrentTargetEnemy;
    private NavMeshAgent m_NavAgent;
    private AudioSource m_AudioSource;
    private float m_LastAttackTime;

    private void Awake()
    {
        ActionName = "Aggressive Engage / Focus Fire";

        if (!TryGetComponent(out m_AudioSource))
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.spatialBlend = 1.0f; // Enable 3D sound
            m_AudioSource.playOnAwake = false;
        }
    }

    public override float EvaluateUtility(PlayerProfile profile, Transform agent, Transform player)
    {
        if (player == null || profile == null) return 0.0f;

        // 1. Locate the closest enemy or the enemy the player is actively facing/attacking
        m_CurrentTargetEnemy = FindAggressiveTarget(agent.position, player);
        if (m_CurrentTargetEnemy == null) return 0.0f;

        // 2. Base weight driven directly by Type 8 Challenger (Aggression)
        float baseUtility = profile.Type8_Challenger;

        // 3. Momentum Bonus: Spikes utility if the player is actively moving forward or attacking
        float playerAggressionBonus = profile.AggressionEMA * 0.3f;

        float finalUtility = baseUtility + playerAggressionBonus;

        return Mathf.Clamp01(finalUtility);
    }

    public override void Execute(Transform agent, Transform player)
    {
        // Debug.Log("[AggressiveAttackAction] Executing: Aggressively Engaging Enemy");
        if (m_CurrentTargetEnemy == null) return;

        if (m_NavAgent == null) m_NavAgent = agent.GetComponent<NavMeshAgent>();

        float distanceToEnemy = Vector3.Distance(agent.position, m_CurrentTargetEnemy.position);

        if (m_NavAgent != null && m_NavAgent.isOnNavMesh)
        {
            m_NavAgent.speed = m_MovementSpeed;
            m_NavAgent.stoppingDistance = m_AttackRange - 0.5f;
            m_NavAgent.SetDestination(m_CurrentTargetEnemy.position);
        }
        Debug.Log($"[AggressiveAttackAction] Distance to Enemy: {distanceToEnemy:F2} units. Attack Range: {m_AttackRange:F2} units.");
        if (distanceToEnemy <= m_AttackRange)
        {
            Vector3 direction = (m_CurrentTargetEnemy.position - agent.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                agent.rotation = Quaternion.LookRotation(direction);
            }
            Debug.Log($"[AggressiveAttackAction] Within attack range. Preparing to attack {m_CurrentTargetEnemy.name}.");
            if (Time.time >= m_LastAttackTime + m_AttackCooldown)
            {
                PerformAttack();
                m_LastAttackTime = Time.time;
            }
        }
    }

    private void PerformAttack()
    {
        Debug.Log($"[AggressiveAttackAction] Engaging {m_CurrentTargetEnemy.name} alongside player!");

        if (m_CurrentTargetEnemy.TryGetComponent<EnemyHealth>(out var health))
        {
            health.TakeDamage(m_Damage);
            PlayAttackSound();
            if (health.IsDead)
            {
                m_CurrentTargetEnemy = null;
                if (m_NavAgent != null && m_NavAgent.isOnNavMesh)
                {
                    m_NavAgent.ResetPath();
                }
            }
        }
    }

    public void PlayAttackSound()
    {
        if (m_AttackAudioClip != null && m_AudioSource != null)
        {
            m_AudioSource.PlayOneShot(m_AttackAudioClip, m_AttackAudioVolume);
        }
    }

    private Transform FindAggressiveTarget(Vector3 origin, Transform player)
    {
        Collider[] colliders = Physics.OverlapSphere(origin, m_CombatRadius, m_EnemyLayer);
        Transform closest = null;
        float minDistance = float.MaxValue;

        // Debug.Log($"[AggressiveAttackAction] Found {colliders.Length} potential targets in combat radius.");

        foreach (Collider col in colliders)
        {
            // 1. Tag Check: Ensure the object (or its root) is tagged as "Enemy"
            if (!col.CompareTag("Enemy") && !col.transform.root.CompareTag("Enemy"))
                continue;
            
            // 2. Health Check: Use GetComponentInParent for safety if colliders are on child meshes
            EnemyHealth health = col.GetComponentInParent<EnemyHealth>();
            if (health == null || health.IsDead)
                continue;

            // 3. Distance Check: Target the root transform of the enemy
            Transform enemyRoot = health.transform;
            float dist = Vector3.Distance(origin, enemyRoot.position);

            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemyRoot;
            }
        }

        return closest;
    }
}
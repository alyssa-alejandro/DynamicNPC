using UnityEngine;
using UnityEngine.AI;

public class DrawAggroAction : UtilityAction
{
    [Header("Combat Settings")]
    [SerializeField] private float m_CombatRadius = 15f;
    [SerializeField] private float m_AttackRange = 2.0f;
    [SerializeField] private float m_Damage = 15.0f;
    [SerializeField] private float m_AttackCooldown = 1.0f;
    [SerializeField] private LayerMask m_EnemyLayer;

    [Header("Protection Settings")]
    [Tooltip("If player HP percentage drops below this, protection utility increases.")]
    [Range(0.1f, 1.0f)]
    [SerializeField] private float m_LowHealthThreshold = 0.5f;

    [Header("Navigation Settings")]
    [SerializeField] private float m_MovementSpeed = 5.0f;

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
        ActionName = "Draw Aggro / Protect Player";

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

    // 1. Read Loyalist / Caution score
    float loyalistWeight = profile.Type6_Loyalist;

    // OPTIONAL GATE: If player is strongly Challenger (Type 8) and NOT Loyalist, skip protection entirely
    if (profile.Type8_Challenger > loyalistWeight && profile.Type8_Challenger > 0.6f)
    {
        return 0.0f; 
    }

    // 2. Read Player Health and Attack State
    float healthPercent = 1.0f;
    bool isUnderAttack = false;

    if (player.TryGetComponent<PlayerHealth>(out var playerHealth))
    {
        healthPercent = playerHealth.HealthPercentage;
        isUnderAttack = playerHealth.IsUnderAttack;
    }

    // 3. Locate target (prioritize enemies attacking the player)
    m_CurrentTargetEnemy = FindPriorityTarget(agent.position, player);
    if (m_CurrentTargetEnemy == null) return 0.0f;

    // 4. Calculate Urgency Multiplier
    float dangerMultiplier = 1.0f;

    if (isUnderAttack)
    {
        dangerMultiplier += 0.5f;
    }

    if (healthPercent <= m_LowHealthThreshold)
    {
        dangerMultiplier += (1.0f - healthPercent);
    }

    // MULTIPLY the loyalist weight instead of adding fixed flat values!
    // If loyalistWeight is low (e.g. 0.1), dangerMultiplier (1.5) only increases score to 0.15.
    float finalUtility = loyalistWeight * dangerMultiplier;

    return Mathf.Clamp01(finalUtility);
}

    public override void Execute(Transform agent, Transform player)
    {
        // Debug.Log("[DrawAggroAction] Executing: Drawing Aggro to Protect Player");
        if (m_CurrentTargetEnemy == null) return;

        if (m_NavAgent == null) m_NavAgent = agent.GetComponent<NavMeshAgent>();

        float distanceToEnemy = Vector3.Distance(agent.position, m_CurrentTargetEnemy.position);

        if (m_NavAgent != null && m_NavAgent.isOnNavMesh)
        {
            m_NavAgent.speed = m_MovementSpeed;
            m_NavAgent.stoppingDistance = m_AttackRange;
            m_NavAgent.SetDestination(m_CurrentTargetEnemy.position);
        }

        if (distanceToEnemy <= m_AttackRange)
        {
            Vector3 direction = (m_CurrentTargetEnemy.position - agent.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                agent.rotation = Quaternion.LookRotation(direction);
            }

            if (Time.time >= m_LastAttackTime + m_AttackCooldown)
            {
                PerformAttack();
                m_LastAttackTime = Time.time;
            }
        }
    }

    private void PerformAttack()
    {
        Debug.Log($"[DrawAggroAction] Intercepting {m_CurrentTargetEnemy.name} to peel off player!");

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


    private Transform FindPriorityTarget(Vector3 origin, Transform player)
    {
        // PRIORITY 1: Check if the player is being actively attacked
        if (player != null && player.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            if (playerHealth.IsUnderAttack && playerHealth.LastAttacker != null)
            {
                // Check if the attacker has the "Enemy" tag
                if (playerHealth.LastAttacker.CompareTag("Enemy"))
                {
                    if (playerHealth.LastAttacker.TryGetComponent<EnemyHealth>(out var attackerHealth) && !attackerHealth.IsDead)
                    {
                        float distToAttacker = Vector3.Distance(origin, playerHealth.LastAttacker.position);
                        if (distToAttacker <= m_CombatRadius * 1.5f) // Slightly larger reach for rescue
                        {
                            return playerHealth.LastAttacker;
                        }
                    }
                }
            }
        }

        return null;
    }
}
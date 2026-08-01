using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Enemy Territory")]
    [SerializeField] private float territoryRadius = 15f;
    [SerializeField] private float homeStoppingDistance = 0.3f;

    [Header("Attack")]
    [SerializeField] private float shootingRange = 4f;
    [SerializeField] private float damagePerShot = 10f;
    [SerializeField] private float shootingCooldown = 1f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip m_AttackAudioClip;
    [Range(0f, 1f)] [SerializeField] private float m_AttackAudioVolume = 0.8f;

    private NavMeshAgent agent;
    private AudioSource m_AudioSource;

    private Transform currentTarget;
    private bool isAggroedByCompanion = false;


    private Vector3 homePosition;
    private float nextShotTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        homePosition = transform.position;

        if (!TryGetComponent(out m_AudioSource))
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.spatialBlend = 1.0f; // Enable 3D sound
            m_AudioSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("EnemyAI has no player assigned.");
            return;
        }

    }
    
    private void Update()
    {
        if (player == null || !agent.isOnNavMesh)
            return;

        float playerDistanceFromHome =
            Vector3.Distance(player.position, homePosition);

        float targetDistanceFromHome = float.MaxValue;
        if (currentTarget != null)
            targetDistanceFromHome = Vector3.Distance(currentTarget.position, homePosition);

        if (playerDistanceFromHome > territoryRadius && targetDistanceFromHome > territoryRadius)
        {
            ReturnHome();
            return;
        }

        if (isAggroedByCompanion && currentTarget != null)
        {
            float distanceToTarget =
                Vector3.Distance(transform.position, currentTarget.position);
                if (distanceToTarget > shootingRange)
                {
                    FollowTarget();
                }
                else
                {
                    AttackTarget();
                }
        }

        else {
        float distanceToPlayer =
                    Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > shootingRange)
            {
                FollowPlayer();
            }
            else
            {
                AttackPlayer();
            }
        }
                
    }

    private void FollowPlayer()
    {
        agent.isStopped = false;
        agent.stoppingDistance = shootingRange * 0.8f;

        // Updated every frame, so the enemy follows the moving player.
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.isStopped = true;

        FaceTarget(player);

        if (Time.time < nextShotTime)
            return;

        nextShotTime = Time.time + shootingCooldown;

        Debug.Log("Enemy shot the player.");

        if (player.TryGetComponent<PlayerHealth>(out var health))
        {
            health.TakeDamage(damagePerShot, transform);
            PlayAttackSound();
            if (health.IsDead)
            {
                player = null;
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.ResetPath();
                }
            }
        }
    }

    private void FollowTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = shootingRange * 0.8f;

        // Updated every frame, so the enemy follows the moving player.
        agent.SetDestination(currentTarget.position);
    }

    private void AttackTarget()
    {
        agent.isStopped = true;

        FaceTarget(currentTarget);

        if (Time.time < nextShotTime)
            return;

        nextShotTime = Time.time + shootingCooldown;

        Debug.Log("Enemy shot the target.");

        PlayAttackSound();
            
    }
    

    public void PlayAttackSound()
    {
        if (m_AttackAudioClip != null && m_AudioSource != null)
        {
            m_AudioSource.PlayOneShot(m_AttackAudioClip, m_AttackAudioVolume);
        }
    }

    private void ReturnHome()
    {
        float distanceFromHome =
            Vector3.Distance(transform.position, homePosition);

        if (distanceFromHome <= homeStoppingDistance)
        {
            agent.isStopped = true;
            agent.ResetPath();
            return;
        }

        agent.isStopped = false;
        agent.stoppingDistance = homeStoppingDistance;
        agent.SetDestination(homePosition);
        isAggroedByCompanion = false;
        currentTarget = null;
    }

    private void FaceTarget(Transform target)
    {
        Vector3 direction =
            target.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation =
                Quaternion.LookRotation(direction);
        }
    }

    public void Aggro(Transform newTarget)
    {
        if (newTarget == null) return;

        currentTarget = newTarget;
        isAggroedByCompanion = true;
        // aggroResetTime = Time.time + m_AggroDuration;

        Debug.Log($"[EnemyAI] {gameObject.name} AGGRO SWAPPED! Now attacking: {newTarget.name}");
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 centre = Application.isPlaying
            ? homePosition
            : transform.position;

        Gizmos.DrawWireSphere(centre, territoryRadius);
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
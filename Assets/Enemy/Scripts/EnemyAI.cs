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

    private NavMeshAgent agent;
    private Health playerHealth;

    private Vector3 homePosition;
    private float nextShotTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        homePosition = transform.position;
    }

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("EnemyAI has no player assigned.");
            return;
        }

        playerHealth = player.GetComponent<Health>();

        if (playerHealth == null)
            playerHealth = player.GetComponentInParent<Health>();

        if (playerHealth == null)
            playerHealth = player.GetComponentInChildren<Health>();

        if (playerHealth == null)
        {
            Debug.LogError(
                $"No Health component found on {player.name}, its parent, or its children."
            );
        }
    }
    
    private void Update()
    {
        if (player == null || !agent.isOnNavMesh)
            return;

        float playerDistanceFromHome =
            Vector3.Distance(player.position, homePosition);

        if (playerDistanceFromHome > territoryRadius)
        {
            ReturnHome();
            return;
        }

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

        FacePlayer();

        if (Time.time < nextShotTime)
            return;

        nextShotTime = Time.time + shootingCooldown;

        Debug.Log("Enemy shot the player.");

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damagePerShot);
        }
        else
        {
            Debug.LogWarning(
                "Enemy cannot damage player because no Health component was found."
            );
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
    }

    private void FacePlayer()
    {
        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation =
                Quaternion.LookRotation(direction);
        }
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
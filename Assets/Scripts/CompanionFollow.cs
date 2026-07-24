using UnityEngine;
using UnityEngine.AI;

public class CompanionFollow : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target != null)
        {
            // agent.SetDestination(target.position);
            // Debug.Log($"Target position: {target.position}");
        }
    }
}

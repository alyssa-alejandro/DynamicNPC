using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Defend or distract", story: "Defends/Distracts [Enemy]", category: "Action", id: "84f256546237543dc716cc78ad27be14")]
public partial class DefendOrDistractAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    [SerializeReference] public BlackboardVariable<GameObject> Self; // Optional reference if needed

    [Header("Behavior Settings")]
    public float StoppingDistance = 3.0f;
    public float DistractSpeed = 5.0f;
    public string DistractAnimationParam = "IsDistracting";

    private NavMeshAgent m_NavAgent;
    private Animator m_Animator;
    private Transform m_SelfTransform;

    protected override Status OnStart()
    {
        // 1. Validation check
        if (Enemy == null || Enemy.Value == null)
        {
            Debug.LogWarning("[DefendOrDistractAction] Enemy reference is missing.");
            return Status.Failure;
        }

        // Get transform reference (either from Blackboard 'Self' or GameObject context)
        GameObject selfObj = (Self != null && Self.Value != null) ? Self.Value : GameObject;
        if (selfObj == null)
        {
            return Status.Failure;
        }

        m_SelfTransform = selfObj.transform;
        m_NavAgent = selfObj.GetComponent<NavMeshAgent>();
        m_Animator = selfObj.GetComponentInChildren<Animator>();

        // 2. Setup NavMeshAgent parameters
        if (m_NavAgent != null && m_NavAgent.isActiveAndEnabled && m_NavAgent.isOnNavMesh)
        {
            m_NavAgent.speed = DistractSpeed;
            m_NavAgent.stoppingDistance = StoppingDistance;
            m_NavAgent.isStopped = false;
            m_NavAgent.SetDestination(Enemy.Value.transform.position);
        }

        // 3. Trigger Distract Animation state
        if (m_Animator != null)
        {
            m_Animator.SetBool(DistractAnimationParam, true);
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // Fail gracefully if target disappears/dies mid-action
        if (Enemy == null || Enemy.Value == null)
        {
            return Status.Failure;
        }

        Transform targetTransform = Enemy.Value.transform;
        float distanceToEnemy = Vector3.Distance(m_SelfTransform.position, targetTransform.position);

        // Update NavMesh destination as enemy moves
        if (m_NavAgent != null && m_NavAgent.isActiveAndEnabled && m_NavAgent.isOnNavMesh)
        {
            m_NavAgent.SetDestination(targetTransform.position);
        }

        // Face the target when in range
        if (distanceToEnemy <= StoppingDistance + 0.5f)
        {
            Vector3 direction = (targetTransform.position - m_SelfTransform.position).normalized;
            direction.y = 0; // Keep rotation level on ground
            if (direction != Vector3.zero)
            {
                m_SelfTransform.rotation = Quaternion.Slerp(
                    m_SelfTransform.rotation, 
                    Quaternion.LookRotation(direction), 
                    Time.deltaTime * 5f
                );
            }
            return Status.Success;
        }

        // Keep running to allow higher priority branches (or tree interrupts) to abort it
        return Status.Running;
    }

    protected override void OnEnd()
    {
        // Stop movement safely
        if (m_NavAgent != null && m_NavAgent.isActiveAndEnabled && m_NavAgent.isOnNavMesh)
        {
            m_NavAgent.ResetPath();
        }

        // Reset animation trigger
        if (m_Animator != null)
        {
            m_Animator.SetBool(DistractAnimationParam, false);
        }

        m_NavAgent = null;
        m_Animator = null;
        m_SelfTransform = null;
    }
}
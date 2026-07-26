using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set CTargetEnemy to AttackingEnemy", story: "Set [ActiveEnemy] from [NavigatedEnemy]", category: "Action", id: "f9b3f0a6dc2b0021c27c060a011f160a")]
public partial class SetTargetToAttackingEnemyAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> ActiveEnemy;    // Points to CTargetedEnemy
    [SerializeReference] public BlackboardVariable<GameObject> NavigatedEnemy; // Points to AttackingEnemy

    protected override Status OnStart()
    {
        // 1. Validation check
        if (NavigatedEnemy == null || NavigatedEnemy.Value == null)
        {
            Debug.LogWarning("[SetTargetToAttackingEnemyAction] NavigatedEnemy is null or unassigned.");
            return Status.Failure;
        }

        if (ActiveEnemy == null)
        {
            Debug.LogError("[SetTargetToAttackingEnemyAction] ActiveEnemy BlackboardVariable is not assigned in the Inspector.");
            return Status.Failure;
        }

        // 2. Assign the reference
        ActiveEnemy.Value = NavigatedEnemy.Value;
        NavigatedEnemy.Value = null; // Clear the reference to avoid confusion

        // Return Success immediately since assignment takes 1 frame
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

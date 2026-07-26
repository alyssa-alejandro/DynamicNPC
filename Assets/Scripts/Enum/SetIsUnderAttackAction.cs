using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set IsUnderAttack", story: "Set [IsUnderAttack] to [Value]", category: "Action", id: "87c675a1b55ce6d7e35596fed4a75672")]
public partial class SetIsUnderAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> IsUnderAttack;
    [SerializeReference] public BlackboardVariable<bool> Value;

    protected override Status OnStart()
    {
        // 1. Validation checks
        if (IsUnderAttack == null)
        {
            Debug.LogError("[SetIsUnderAttackAction] IsUnderAttack BlackboardVariable is not assigned in the Inspector.");
            return Status.Failure;
        }

        if (Value == null)
        {
            Debug.LogError("[SetIsUnderAttackAction] Value BlackboardVariable is not assigned in the Inspector.");
            return Status.Failure;
        }

        // 2. Assign the boolean value
        IsUnderAttack.Value = Value.Value;

        // Return Success immediately so the node completes in 1 frame
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


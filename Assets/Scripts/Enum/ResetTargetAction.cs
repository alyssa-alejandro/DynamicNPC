using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Reset Target", story: "Set [Target] to null", category: "Action", id: "e2aefd68cf0479ee5645e0fc7f19fff3")]
public partial class ResetTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        if (Target == null)
        {
            Debug.LogError("[ResetTargetAction] Target BlackboardVariable is not assigned in the Inspector.");
            return Status.Failure;
        }

        Target.Value = null;
        return Status.Failure;
    }

    protected override Status OnUpdate()
    {
        return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}


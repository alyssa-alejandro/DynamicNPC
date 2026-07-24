using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetMovementSpeed", story: "Set [Object] speed to [Value]", category: "Action", id: "80bc375f1bc8d95ba238bf9b3d92662d")]
public partial class SetMovementSpeedAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Object;
    [SerializeReference] public BlackboardVariable<float> Value;

    protected override Status OnStart()
    {
        // 1. Safety check: Ensure both variables and the GameObject exist
        if (Object == null || Object.Value == null || Value == null)
        {
            return Status.Failure;
        }

        // 2. Grab the NavMeshAgent component from the target GameObject
        if (Object.Value.TryGetComponent<NavMeshAgent>(out var agent))
        {
            // 3. Set the new movement speed
            agent.speed = Value.Value;
            return Status.Success;
        }

        // Return failure if the GameObject doesn't have a NavMeshAgent
        Debug.LogWarning($"SetMovementSpeedAction: No NavMeshAgent found on {Object.Value.name}");
        return Status.Failure;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}
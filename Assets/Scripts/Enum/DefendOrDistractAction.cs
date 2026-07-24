using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Defend or distract", story: "Defends/Distracts [Enemy]", category: "Action", id: "84f256546237543dc716cc78ad27be14")]
public partial class DefendOrDistractAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;

    protected override Status OnStart()
    {
        Debug.Log($"Defend or distract {Enemy.Value.name}");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Debug.Log($"Defend or distract {Enemy.Value.name}");
        return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}


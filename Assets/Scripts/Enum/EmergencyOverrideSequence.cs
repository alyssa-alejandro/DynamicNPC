using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Emergency Override", story: "Immediately execute if [PLAYER] in danger", category: "Flow", id: "b8d230762cf5d0b0cabf8fe1b6d5cb5f")]
public partial class EmergencyOverrideSequence : Composite
{
    [SerializeReference] public BlackboardVariable<GameObject> PLAYER;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}


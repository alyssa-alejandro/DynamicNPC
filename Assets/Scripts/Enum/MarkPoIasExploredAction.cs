using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MarkPOIasExplored", story: "Mark [POI] as Explored", category: "Action", id: "5c11dffaaf32b8d666bac204e190c0c8")]
public partial class MarkPOIExploredAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> POI;

    protected override Status OnStart()
    {
        if (POI == null || POI.Value == null)
        {
            Debug.LogWarning("[MarkPOIExploredAction] TargetPOI is null or unassigned.");
            return Status.Failure;
        }

        // Try to get the PointOfInterest component from the target chest
        if (POI.Value.TryGetComponent<PointOfInterest>(out var poi))
        {
            poi.MarkAsExplored(); // Sets isExplored = true
            
            // Clear the Blackboard variable so it doesn't hold onto the old target
            POI.Value = null;

            return Status.Success;
        }

        Debug.LogError($"[MarkPOIExploredAction] {POI.Value.name} does not have a PointOfInterest component!");
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


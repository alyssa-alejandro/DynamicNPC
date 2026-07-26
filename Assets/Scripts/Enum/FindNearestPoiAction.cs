using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindNearestPOI", story: "Find nearest unexplored [POI] for [Self]", category: "Action", id: "b81c0b56f0cd2ea49de68853e0c64c10")]
public partial class FindNearestPOIAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> POI;

    [Header("Exploration Settings")]
    public float MaxDistance = 15.0f; // Maximum detection range for chests

    protected override Status OnStart()
    {
        GameObject selfObj = (Self != null && Self.Value != null) ? Self.Value : GameObject;
        if (selfObj == null || POI == null)
            return Status.Failure;

        // Find all POIs in the scene
        PointOfInterest[] allPOIs = UnityEngine.Object.FindObjectsByType<PointOfInterest>(FindObjectsSortMode.None);

        PointOfInterest closest = null;
        float minDistance = MaxDistance; // Restrict search within MaxDistance range

        foreach (PointOfInterest poi in allPOIs)
        {
            // Skip chests that are already opened/explored
            if (poi.isExplored) continue;

            float dist = Vector3.Distance(selfObj.transform.position, poi.transform.position);
            if (dist <= minDistance)
            {
                minDistance = dist;
                closest = poi;
            }
        }

        if (closest != null)
        {
            POI.Value = closest.gameObject;
            return Status.Success;
        }

        // No unexplored POI found within MaxDistance
        POI.Value = null;
        return Status.Failure;
    }
}


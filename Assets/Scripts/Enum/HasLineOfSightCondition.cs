using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Has LineOfSight", story: "[Agent] has LineOfSight to [Target]", category: "Conditions", id: "847197f130b798f60b65f8fc435e78f6")]
public partial class HasLineOfSightCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    // Use standard LayerMask instead of BlackboardVariable<LayerMask>
    public LayerMask ObstacleMask = -1; // Default to all layers

    public override bool IsTrue()
    {
        
        GameObject selfObj = (Agent != null && Agent.Value != null) ? Agent.Value : GameObject;
        if (selfObj == null || Target == null || Target.Value == null)
            return false;

        Vector3 start = selfObj.transform.position;
        Vector3 targetPos = Target.Value.transform.position;
        Vector3 direction = targetPos - start;

        Debug.DrawLine(start, targetPos, Color.red, 1.0f);
        // Perform raycast check using ObstacleMask
        if (Physics.Raycast(start, direction.normalized, out RaycastHit hit, direction.magnitude, ObstacleMask))
        {
            // If we hit something other than target, line of sight is blocked
            if (hit.transform.gameObject != Target.Value && !hit.transform.IsChildOf(Target.Value.transform))
            {
                Debug.Log($"Line of sight blocked by {hit.transform.gameObject.name} between {selfObj.name} and {Target.Value.name}");
                return false;
            }
        }
        Debug.Log($"Line of sight clear between {selfObj.name} and {Target.Value.name}");
        return true;
    }
}
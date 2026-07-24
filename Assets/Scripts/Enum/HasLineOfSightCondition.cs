using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Has LineOfSight", story: "[Agent] has LineOfSight to [Target]", category: "Conditions", id: "847197f130b798f60b65f8fc435e78f6")]
public partial class HasLineOfSightCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<LayerMask> ObstacleMask; 

    public override bool IsTrue()
    {
        // 1. Safety checks: Ensure both GameObjects exist
        if (Agent == null || Agent.Value == null || Target == null || Target.Value == null)
        {
            return false;
        }

        Vector2 startPos = Agent.Value.transform.position;
        Vector2 targetPos = Target.Value.transform.position;

        // Check if ObstacleMask variable is set; if null, default to Everything
        LayerMask mask = (ObstacleMask != null) ? ObstacleMask.Value : (LayerMask)~0;

        // 2. Cast a line from Agent to Target against the ObstacleMask layer
        RaycastHit2D hit = Physics2D.Linecast(startPos, targetPos, mask);

        // 3. If linecast hits an obstacle layer before reaching target, line of sight is blocked
        if (hit.collider != null)
        {
            return false; 
        }

        // Clear line of sight!
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
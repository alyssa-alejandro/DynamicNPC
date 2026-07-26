using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Check Underattack", story: "[Player] [IsUnderAttack]", category: "Conditions", id: "1e04c2463671fe08c9c62954bfaf811b")]
public partial class CheckIsUnderAttackCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<bool> IsUnderAttack;

    public override bool IsTrue()
    {
        if (IsUnderAttack == null)
        {
            return false;
        }

        return IsUnderAttack.Value;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}

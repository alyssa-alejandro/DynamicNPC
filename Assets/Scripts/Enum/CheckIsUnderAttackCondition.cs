using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Check is under attack", story: "Check if [player] [IsUnderAttack]", category: "Conditions", id: "45758658cfb39f5cc97bb41d8de93df4")]
public partial class CheckIsUnderAttackCondition : Condition
{
    [SerializeReference] public BlackboardVariable<bool> Player;
    [SerializeReference] public BlackboardVariable<bool> IsUnderAttack;

    public override bool IsTrue()
    {
        if (Player == null || Player.Value == null || IsUnderAttack == null)
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

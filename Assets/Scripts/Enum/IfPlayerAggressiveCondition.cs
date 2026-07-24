using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "check AGGRESIVE", story: "if [PLAYER] is [AGGRESSIVE]", category: "Conditions", id: "a563d4e98e640c1b1193cf9a8f4c82eb")]
public partial class IfPlayerAggressiveCondition : Condition
{
    [SerializeReference] public BlackboardVariable<PLAYSTYLE> PLAYER;
    [SerializeReference] public BlackboardVariable<PLAYSTYLE> AGGRESSIVE;

    public override bool IsTrue()
    {
        if (PLAYER == null || AGGRESSIVE == null)
        {
            return false;
        }

        return PLAYER.Value.Equals(AGGRESSIVE.Value);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}

using UnityEngine;

public abstract class UtilityAction : MonoBehaviour
{
    [Header("Action Configuration")]
    public string ActionName;

    /// <summary>
    /// Calculates the Utility Score (0.0 to 1.0) based on continuous EMA/Enneagram weights.
    /// </summary>
    public abstract float EvaluateUtility(PlayerProfile profile, Transform agent, Transform player);

    /// <summary>
    /// Executes the action logic when this action wins execution.
    /// </summary>
    public abstract void Execute(Transform agent, Transform player);
}
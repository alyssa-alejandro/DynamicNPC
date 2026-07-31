using System.Collections.Generic;
using UnityEngine;

public class UtilityBrain : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TelemetryManager m_TelemetryTracker;
    [SerializeField] private Transform m_PlayerTransform;

    [Header("Settings")]
    [SerializeField] private float m_EvaluationInterval = 0.2f; // Evaluate 5 times per second

    private List<UtilityAction> m_AvailableActions = new List<UtilityAction>();
    private UtilityAction m_CurrentAction;
    private float m_Timer;

    // For Live Debugging
    public Dictionary<string, float> LastScores { get; private set; } = new Dictionary<string, float>();

    private void Awake()
    {
        // Collect all UtilityAction components attached to this object or children
        m_AvailableActions.AddRange(GetComponentsInChildren<UtilityAction>());
    }

    private void Update()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer >= m_EvaluationInterval)
        {
            m_Timer = 0f;
            EvaluateAndExecute();
        }

        // Run current action frame updates
        if (m_CurrentAction != null)
        {
            m_CurrentAction.Execute(transform, m_PlayerTransform);
        }
    }

    private void EvaluateAndExecute()
    {
        if (m_TelemetryTracker == null) return;

        PlayerProfile profile = m_TelemetryTracker.CurrentProfile;
        UtilityAction bestAction = null;
        float highestUtility = -1.0f;

        LastScores.Clear();

        foreach (UtilityAction action in m_AvailableActions)
        {
            float score = action.EvaluateUtility(profile, transform, m_PlayerTransform);
            LastScores[action.ActionName] = score;

            if (score > highestUtility)
            {
                highestUtility = score;
                bestAction = action;
            }
        }

        if (bestAction != m_CurrentAction)
        {
            m_CurrentAction = bestAction;
            Debug.Log($"[Utility Brain] Switched state to: {m_CurrentAction?.ActionName} (Utility: {highestUtility:F2})");
        }
    }
}
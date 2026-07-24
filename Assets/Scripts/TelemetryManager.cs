using UnityEngine;
using Unity.Behavior; // Required for Unity Behavior Graph API

public class TelemetryManager : MonoBehaviour
{
    [Header("References")]
    public BehaviorGraphAgent companionAgent;
    public Transform playerTransform;

    [Header("Telemetry Settings")]
    public float evalInterval = 2.0f; // Evaluate playstyle every 2 seconds
    
    // Telemetry Metrics
    private float currentNoiseLevel = 0f;
    private float explorationScore = 0f;
    private Vector3 lastPlayerPos;

    public PLAYSTYLE activePlaystyle = PLAYSTYLE.BALANCED;

    void Start()
    {
        if (playerTransform != null) lastPlayerPos = playerTransform.position;
        InvokeRepeating(nameof(EvaluateTelemetry), evalInterval, evalInterval);
    }

    void Update()
    {
        // 1. Calculate continuous noise/movement metric
        if (playerTransform != null)
        {
            float speed = (playerTransform.position - lastPlayerPos).magnitude / Time.deltaTime;
            currentNoiseLevel = Mathf.Clamp01(speed / 8.0f); // Fast movement = higher noise
            lastPlayerPos = playerTransform.position;
        }

        // 2. Sync raw metric to Blackboard for real-time nodes
        if (companionAgent != null)
        {
            companionAgent.SetVariableValue("PlayerNoise", currentNoiseLevel);
        }
    }

    void EvaluateTelemetry()
    {
        // Classify long-term trend
        if (currentNoiseLevel < 0.3f)
        {
            activePlaystyle = PLAYSTYLE.STEALTH;
        }
        else if (currentNoiseLevel > 0.6f)
        {
            activePlaystyle = PLAYSTYLE.AGGRESSIVE;
        }
        else
        {
            activePlaystyle = PLAYSTYLE.BALANCED;
        }

        // Write active profile enum/int to Blackboard
        if (companionAgent != null)
        {
            companionAgent.SetVariableValue("CurrentPlaystyle", (int)activePlaystyle);
        }
    }
}

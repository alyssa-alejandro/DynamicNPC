using UnityEngine;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private TelemetryManager m_TelemetryTracker;

    private Vector2 m_ScrollPosition; // Track scroll position

    private void OnGUI()
    {
        if (m_TelemetryTracker == null) return;
        Matrix4x4 originalMatrix = GUI.matrix;
        float scale = 1.8f; 
        GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1.0f));

        PlayerProfile p = m_TelemetryTracker.CurrentProfile;

        GUILayout.BeginArea(new Rect(20, 20, 320, 380), "Telemetry & Enneagram Live Debug", GUI.skin.window);

        GUILayout.Label($"<b>Dominant Archetype:</b> {p.DominantType}");
        GUILayout.Space(10);

        GUILayout.Label($"Aggression EMA: {p.AggressionEMA:F2}");
        GUILayout.HorizontalSlider(p.AggressionEMA, 0f, 1f);

        GUILayout.Label($"Stealth EMA: {p.StealthEMA:F2}");
        GUILayout.HorizontalSlider(p.StealthEMA, 0f, 1f);

        GUILayout.Label($"Exploration EMA: {p.ExplorationEMA:F2}");
        GUILayout.HorizontalSlider(p.ExplorationEMA, 0f, 1f);

        GUILayout.Label($"Caution EMA: {p.CautionEMA:F2}");
        GUILayout.HorizontalSlider(p.CautionEMA, 0f, 1f);

        GUILayout.Space(10);
        GUILayout.Label("<b>Archetype Weights:</b>");
        GUILayout.Label($"Type 8 (Challenger): {p.Type8_Challenger * 100f:F0}%");
        GUILayout.Label($"Type 5 (Investigator): {p.Type5_Investigator * 100f:F0}%");
        GUILayout.Label($"Type 6 (Loyalist): {p.Type6_Loyalist * 100f:F0}%");
        GUILayout.Label($"Type 1 (Perfectionist): {p.Type1_Perfectionist * 100f:F0}%");

        GUILayout.EndArea();

        GUI.matrix = originalMatrix;
    }
}
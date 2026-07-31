using UnityEngine;

public class EnneagramEvaluator : MonoBehaviour
{
    [SerializeField] private TelemetryManager m_TelemetryTracker;

    private void Update()
    {
        if (m_TelemetryTracker == null) return;

        PlayerProfile profile = m_TelemetryTracker.CurrentProfile;

        // Calculate archetypal fit scores using fuzzy logic combinations
        profile.Type8_Challenger = profile.AggressionEMA * (1.0f - profile.StealthEMA);
        profile.Type5_Investigator = profile.StealthEMA * profile.ExplorationEMA;
        profile.Type6_Loyalist = profile.CautionEMA * (1.0f - profile.AggressionEMA);
        profile.Type1_Perfectionist = profile.ExplorationEMA * (1.0f - profile.CautionEMA);

        // Determine Dominant Type
        DetermineDominantType(profile);
    }

    private void DetermineDominantType(PlayerProfile profile)
    {
        float maxScore = -1f;

        if (profile.Type8_Challenger > maxScore)
        {
            maxScore = profile.Type8_Challenger;
            profile.DominantType = "Type 8: Challenger (Aggressive)";
        }
        if (profile.Type5_Investigator > maxScore)
        {
            maxScore = profile.Type5_Investigator;
            profile.DominantType = "Type 5: Investigator (Stealth)";
        }
        if (profile.Type6_Loyalist > maxScore)
        {
            maxScore = profile.Type6_Loyalist;
            profile.DominantType = "Type 6: Loyalist (Cautious)";
        }
    }
}
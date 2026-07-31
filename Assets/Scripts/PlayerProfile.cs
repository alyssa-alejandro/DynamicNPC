using UnityEngine;

[System.Serializable]
public class PlayerProfile
{
    // Continuous Smoothed EMA Signals (0.0 to 1.0)
    public float AggressionEMA;
    public float StealthEMA;
    public float ExplorationEMA;
    public float CautionEMA;

    // Evaluated Enneagram Weights (0.0 to 1.0)
    public float Type8_Challenger;   // High Combat, Low Stealth
    public float Type5_Investigator; // High Stealth, High Exploration
    public float Type6_Loyalist;     // High Caution, Low Aggression
    public float Type1_Perfectionist;// High Exploration, Low Damage Taken

    // Dominant Archetype for debug/logging
    public string DominantType = "Unclassified";
}
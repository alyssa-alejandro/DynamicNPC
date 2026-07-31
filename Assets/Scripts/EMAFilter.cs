using UnityEngine;

public class EMAFilter
{
    private float m_AttackAlpha; // Smoothing factor when signal goes UP
    private float m_DecayAlpha;  // Smoothing factor when signal goes DOWN
    
    public float CurrentValue { get; private set; }

    // Constructor supporting dual alphas
    public EMAFilter(float attackAlpha, float decayAlpha, float initialValue = 0f)
    {
        m_AttackAlpha = Mathf.Max(0.01f, attackAlpha);
        m_DecayAlpha = Mathf.Max(0.01f, decayAlpha);
        CurrentValue = initialValue;
    }

    public float Update(float newValue, float deltaTime)
    {
        // 1. Pick attack alpha if rising, decay alpha if falling
        float selectedAlpha = (newValue >= CurrentValue) ? m_AttackAlpha : m_DecayAlpha;

        // 2. Apply your framerate-independent formula
        float adjustedAlpha = 1.0f - Mathf.Exp(-selectedAlpha * deltaTime);
        CurrentValue = (newValue * adjustedAlpha) + (CurrentValue * (1.0f - adjustedAlpha));
        
        return CurrentValue;
    }
}
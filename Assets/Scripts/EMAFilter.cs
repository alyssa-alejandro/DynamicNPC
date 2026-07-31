using UnityEngine;

public class EMAFilter
{
    private float m_Alpha; // Smoothing factor (0 < Alpha <= 1). Lower = smoother/slower.
    public float CurrentValue { get; private set; }

    public EMAFilter(float alpha, float initialValue = 0f)
    {
        m_Alpha = Mathf.Clamp01(alpha);
        CurrentValue = initialValue;
    }

    public float Update(float newValue, float deltaTime)
    {
        // Adjust alpha based on frame time for framerate independence
        float adjustedAlpha = 1.0f - Mathf.Exp(-m_Alpha * deltaTime);
        CurrentValue = (newValue * adjustedAlpha) + (CurrentValue * (1.0f - adjustedAlpha));
        return CurrentValue;
    }
}
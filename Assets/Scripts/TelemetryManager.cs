using UnityEngine;

public class TelemetryManager : MonoBehaviour
{
    [Header("EMA Tuning")]
    [Tooltip("Lower values make the companion react slower to habit changes, higher values make it jumpy.")]
    [SerializeField] private float m_EmaAlpha = 0.5f;

    [Header("Telemetry Settings")]
    [SerializeField] private LayerMask m_EnemyLayer;
    [SerializeField] private float m_CombatScanRadius = 12.0f;

    public PlayerProfile CurrentProfile { get; private set; } = new PlayerProfile();

    // Internal Filters
    private EMAFilter m_AggressionFilter;
    private EMAFilter m_StealthFilter;
    private EMAFilter m_ExplorationFilter;
    private EMAFilter m_CautionFilter;

    // Frame Accumulators
    private int m_ValidAttacksThisFrame;
    private bool m_IsCrouching;

    private void Awake()
    {
        m_AggressionFilter = new EMAFilter(m_EmaAlpha);
        m_StealthFilter = new EMAFilter(m_EmaAlpha);
        m_ExplorationFilter = new EMAFilter(m_EmaAlpha);
        m_CautionFilter = new EMAFilter(m_EmaAlpha);
    }

    public void RecordAttackIntent()
    {
        m_ValidAttacksThisFrame++;
    }

    public void SetCrouchState(bool isCrouching)
    {
        m_IsCrouching = isCrouching;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // 1. Calculate Aggression (Attacks performed WHILE enemies are nearby)
        bool enemiesNearby = Physics.CheckSphere(transform.position, m_CombatScanRadius, m_EnemyLayer);
        float rawAggression = (enemiesNearby && m_ValidAttacksThisFrame > 0) ? 1.0f : 0.0f;
        CurrentProfile.AggressionEMA = m_AggressionFilter.Update(rawAggression, dt);

        // 2. Calculate Stealth (Crouching state + low movement noise)
        float rawStealth = m_IsCrouching ? 1.0f : 0.0f;
        CurrentProfile.StealthEMA = m_StealthFilter.Update(rawStealth, dt);

        // Reset accumulators
        m_ValidAttacksThisFrame = 0;
    }
}
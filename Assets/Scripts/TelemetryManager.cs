using UnityEngine;
using StarterAssets;

public class TelemetryManager : MonoBehaviour
{
    [Header("EMA Tuning")]
    [Tooltip("Lower values make the companion react slower to habit changes, higher values make it jumpy.")]
    [SerializeField] private float m_EmaAlpha = 0.5f;

    [Header("Telemetry Settings")]
    [SerializeField] private LayerMask m_EnemyLayer;
    [SerializeField] private float m_CombatScanRadius = 12.0f;

    [Header("Player Settings")]
    [SerializeField] private float m_SprintPlayerSpeed =  5.335f; // Set this to your SPRINT speed
    [SerializeField] private float m_WalkPlayerSpeed = 2.0f; // Set this to your WALK speed

    [Header("Telemetry Weight Settings")]
    [SerializeField] private float m_RetreatCautionWeight = 0.8f;
    

    public PlayerProfile CurrentProfile { get; private set; } = new PlayerProfile();

    // Internal Filters
    private EMAFilter m_AggressionFilter;
    private EMAFilter m_StealthFilter;
    private EMAFilter m_ExplorationFilter;
    private EMAFilter m_CautionFilter;

    // Frame Accumulators
    private int m_ValidAttacksThisFrame;
    private bool m_IsCrouching;
    private Vector3 _lastPosition;
    
    private StarterAssetsInputs _input;

    private void Awake()
    {
        // EMAFilter(AttackAlpha, DecayAlpha)
        // High attack (e.g. 2.0 to 5.0) = builds quickly when performing the action
        // Low decay (e.g. 0.1 to 0.5) = drops VERY slowly when idle
        
        m_AggressionFilter  = new EMAFilter(3.0f, 0.13f);
        m_StealthFilter     = new EMAFilter(3.0f, 0.05f);
        m_ExplorationFilter = new EMAFilter(2.0f, 0.08f);
        m_CautionFilter     = new EMAFilter(3.0f, 0.05f);

        if (_input == null) 
            _input = GetComponent<StarterAssetsInputs>();
    }
    public void RecordAttackIntent()
    {
        m_ValidAttacksThisFrame++;
    }

    public void RecordCompanionProximity()
    {
        // Feeds input into your Caution EMA filter when staying near the companion
        if (m_CautionFilter != null)
        {
            m_CautionFilter.Update(Time.deltaTime, 1.0f); 
        }
    }

    public void RecordBackingAway(float deltaTime)
    {
        // Feed caution input into the EMA filter
        if (m_CautionFilter != null)
        {
            // 1.0f signals active backing away intent
            float inputSignal = 1.0f * m_RetreatCautionWeight;
            m_CautionFilter.Update(deltaTime, inputSignal);
        }

        if (m_AggressionFilter != null)
        {
            m_AggressionFilter.Update(deltaTime, 0.1f);
        }
    }

    public void SetCrouchState(bool isCrouching)
    {
        m_IsCrouching = isCrouching;
    }

    private void Start()
    {
        _lastPosition = transform.position;
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

        // ⚡ 3. Calculate Exploration (Movement speed / discovering space)
        float distanceMoved = Vector3.Distance(transform.position, _lastPosition);
        _lastPosition = transform.position;

        // Player gets high exploration raw signal when moving/exploring, zero when standing still
        float targetSpeed = _input != null && _input.sprint ? m_SprintPlayerSpeed : m_WalkPlayerSpeed;
        float rawExploration = Mathf.Clamp01(distanceMoved / (targetSpeed * dt)); 
        CurrentProfile.ExplorationEMA = m_ExplorationFilter.Update(rawExploration, dt);

        // ⚡ 4. Calculate Caution (Standing still / scanning environment while crouching)
        float rawCaution = (m_IsCrouching && distanceMoved < 0.1f * dt) ? 1.0f : 0.0f;
        CurrentProfile.CautionEMA = m_CautionFilter.Update(rawCaution, dt);

        // Reset accumulators
        m_ValidAttacksThisFrame = 0;
    }
}
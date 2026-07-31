using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float m_MaxHealth = 100f;
    [SerializeField] private float m_CurrentHealth = 100f;
    [SerializeField] private float m_UnderAttackWindow = 3.0f;
    
    [Header("UI Reference")]
    [SerializeField] private WorldSpaceHealthBar m_HealthBar;

    private float m_LastDamageTime = -999f;
    public Transform LastAttacker { get; private set; }

    public float HealthPercentage => Mathf.Clamp01(m_CurrentHealth / m_MaxHealth);
    public bool IsUnderAttack => (Time.time - m_LastDamageTime) <= m_UnderAttackWindow;

    private void Start()
    {
        UpdateHealthUI();
    }

    public void TakeDamage(float amount, Transform attacker = null)
    {
        m_CurrentHealth = Mathf.Max(0f, m_CurrentHealth - amount);
        m_LastDamageTime = Time.time;
        
        if (attacker != null) LastAttacker = attacker;

        UpdateHealthUI();
        Debug.Log($"[PlayerHealth] Took {amount} damage! HP: {m_CurrentHealth}/{m_MaxHealth}");
    }

    public void Heal(float amount)
    {
        m_CurrentHealth = Mathf.Min(m_MaxHealth, m_CurrentHealth + amount);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (m_HealthBar != null)
        {
            m_HealthBar.SetHealthPercent(HealthPercentage);
        }
    }
}
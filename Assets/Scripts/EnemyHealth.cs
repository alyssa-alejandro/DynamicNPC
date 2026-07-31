using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float m_MaxHealth = 50f;
    [SerializeField] private WorldSpaceHealthBar m_HealthBar;

    private float m_CurrentHealth;

    public bool IsDead => m_CurrentHealth <= 0f;

    private void Awake()
    {
        m_CurrentHealth = m_MaxHealth;
    }

    private void Start()
    {
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        m_CurrentHealth -= amount;
        UpdateHealthUI();

        Debug.Log($"[EnemyHealth] {gameObject.name} took {amount} damage! HP: {m_CurrentHealth}");

        if (m_CurrentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (m_HealthBar != null)
        {
            m_HealthBar.SetHealthPercent(m_CurrentHealth / m_MaxHealth);
        }
    }

    private void Die()
    {
        Debug.Log($"[EnemyHealth] {gameObject.name} died!");
        Destroy(gameObject, 0.1f);
    }
}
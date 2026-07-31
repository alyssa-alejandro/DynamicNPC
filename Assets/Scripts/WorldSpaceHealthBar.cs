using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image m_FillImage;
    [SerializeField] private Vector3 m_Offset = new Vector3(0f, 2.2f, 0f);

    private Transform m_TargetTransform;
    private Camera m_MainCamera;

    private void Start()
    {
        m_MainCamera = Camera.main;
        m_TargetTransform = transform.parent; // Expects to be parented to Player/Enemy
    }

    private void LateUpdate()
    {
        if (m_TargetTransform == null) return;

        // 1. Maintain height offset relative to parent entity
        transform.position = m_TargetTransform.position + m_Offset;

        // 2. Billboard effect: Always face the camera so it doesn't look flat/skewed
        if (m_MainCamera != null)
        {
            transform.rotation = m_MainCamera.transform.rotation;
        }
    }

    /// <summary>
    /// Call this to update the fill percentage (0.0 to 1.0)
    /// </summary>
    public void SetHealthPercent(float normalizedHealth)
    {
        if (m_FillImage != null)
        {
            m_FillImage.fillAmount = Mathf.Clamp01(normalizedHealth);
        }
    }
}
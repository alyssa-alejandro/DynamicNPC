using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    [Header("POI Settings")]
    public string poiType = "Chest";
    public bool isExplored = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip m_ExploreAudioClip;
    [Range(0f, 1f)] [SerializeField] private float m_ExploreAudioVolume = 0.8f;

    private AudioSource m_AudioSource;

    private void Awake()
    {
        if (!TryGetComponent(out m_AudioSource))
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.spatialBlend = 1.0f; // Enable 3D sound
            m_AudioSource.playOnAwake = false;
        }
    }

    // Call this when the companion reaches or opens the chest
    public void MarkAsExplored()
    {
        isExplored = true;
        PlayExploreSound();
    }

    public void PlayExploreSound()
    {
        if (m_ExploreAudioClip != null && m_AudioSource != null)
        {
            m_AudioSource.PlayOneShot(m_ExploreAudioClip, m_ExploreAudioVolume);
        }
    }

}
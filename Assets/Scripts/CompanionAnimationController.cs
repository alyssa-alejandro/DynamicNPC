using UnityEngine;
using UnityEngine.AI;

public class CompanionAnimationController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Smoothing time for animation speed transitions")]
    [SerializeField] private float m_SpeedChangeRate = 10.0f;

    [Header("Audio Settings")]
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Header("Target References")]
    [SerializeField] private Transform m_PlayerTransform;

    // References
    private NavMeshAgent m_NavAgent;
    private Animator m_Animator;

    // Animator parameter hash IDs (matches ThirdPersonController)
    private int m_AnimIDSpeed;
    private int m_AnimIDGrounded;
    private int m_AnimIDJump;
    private int m_AnimIDFreeFall;
    private int m_AnimIDMotionSpeed;

    private float m_AnimationBlend;
    private bool m_HasAnimator;

    private void Awake()
    {
        m_NavAgent = GetComponent<NavMeshAgent>();
        m_HasAnimator = TryGetComponent(out m_Animator);

        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        m_AnimIDSpeed = Animator.StringToHash("Speed");
        m_AnimIDGrounded = Animator.StringToHash("Grounded");
        m_AnimIDJump = Animator.StringToHash("Jump");
        m_AnimIDFreeFall = Animator.StringToHash("FreeFall");
        m_AnimIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void Update()
    {
        if (!m_HasAnimator || m_NavAgent == null) return;

        UpdateMovementAnimations();
        UpdateGroundedState();
    }

    private void UpdateMovementAnimations()
    {
        // 1. Get current movement speed on the horizontal plane (ignoring Y/vertical movement)
        Vector3 horizontalVelocity = new Vector3(m_NavAgent.velocity.x, 0f, m_NavAgent.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        // 2. Smoothly lerp the speed parameter for fluid transitions
        m_AnimationBlend = Mathf.Lerp(m_AnimationBlend, currentSpeed, Time.deltaTime * m_SpeedChangeRate);
        if (m_AnimationBlend < 0.01f) m_AnimationBlend = 0f;

        // 3. Update Animator floats
        m_Animator.SetFloat(m_AnimIDSpeed, m_AnimationBlend);
        
        // Keep MotionSpeed at 1.0f while moving so clips play at normal playback speed
        float motionSpeedMultiplier = currentSpeed > 0.1f ? 1.0f : 0.0f;
        m_Animator.SetFloat(m_AnimIDMotionSpeed, motionSpeedMultiplier);
    }

    private void UpdateGroundedState()
    {
        // Check OffMeshLinks / NavMesh state to flag off-ground status
        bool isGrounded = m_NavAgent.isOnNavMesh && !m_NavAgent.isOnOffMeshLink;

        m_Animator.SetBool(m_AnimIDGrounded, isGrounded);
        m_Animator.SetBool(m_AnimIDJump, false);
        m_Animator.SetBool(m_AnimIDFreeFall, !isGrounded);
    }

    /// <summary>
    /// Call this from Utility Actions (like DrawAggroAction) to trigger an attack animation.
    /// </summary>
    public void TriggerAttackAnimation()
    {
        if (m_HasAnimator)
        {
            m_Animator.SetTrigger("Attack"); // Make sure your Animator Controller has an "Attack" trigger!
        }
    }
    
    // Receiver for OnFootstep Animation Event
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips != null && FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
            }
        }
    }

    // Receiver for OnLand Animation Event
    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f && LandingAudioClip != null)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.position, FootstepAudioVolume);
        }
    }
}
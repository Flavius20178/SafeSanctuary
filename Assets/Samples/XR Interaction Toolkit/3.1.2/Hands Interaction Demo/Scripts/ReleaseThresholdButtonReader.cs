using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    ///     An input button reader based on another <see cref="XRInputButtonReader" /> and holds it true until falling below a
    ///     lower release threshold.
    ///     Useful with hand interaction because the bool select value can bounce when the hand is near the tight internal
    ///     threshold,
    ///     so using this will keep the pinch true until moving the fingers much further away than the pinch activation
    ///     threshold.
    /// </summary>
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_XRInputDeviceButtonReader)]
    public class ReleaseThresholdButtonReader : MonoBehaviour, IXRInputButtonReader
    {
        [SerializeField] [Tooltip("The source input that this component reads to create a processed button value.")]
        private XRInputButtonReader m_ValueInput = new("Value");

        [SerializeField]
        [Tooltip(
            "The threshold value to use to determine when the button is pressed. Considered pressed equal to or greater than this value.")]
        [Range(0f, 1f)]
        private float m_PressThreshold = 0.8f;

        [SerializeField]
        [Tooltip(
            "The threshold value to use to determine when the button is released when it was previously pressed. Keeps being pressed until falls back to a value of or below this value.")]
        [Range(0f, 1f)]
        private float m_ReleaseThreshold = 0.25f;

        private bool m_IsPerformed;
        private bool m_WasCompletedThisFrame;
        private bool m_WasPerformedThisFrame;

        /// <summary>
        ///     The source input that this component reads to create a processed button value.
        /// </summary>
        public XRInputButtonReader valueInput
        {
            get => m_ValueInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_ValueInput, value, this);
        }

        /// <summary>
        ///     The threshold value to use to determine when the button is pressed. Considered pressed equal to or greater than
        ///     this value.
        /// </summary>
        /// <remarks>
        ///     This reader will also be considered performed if the source input is performed.
        /// </remarks>
        public float pressThreshold
        {
            get => m_PressThreshold;
            set => m_PressThreshold = value;
        }

        /// <summary>
        ///     The threshold value to use to determine when the button is released when it was previously pressed.
        ///     Keeps being pressed until falls back to a value of or below this value.
        /// </summary>
        /// <remarks>
        ///     This reader will still be considered performed if the source input is still performed
        ///     when this threshold is reached.
        /// </remarks>
        public float releaseThreshold
        {
            get => m_ReleaseThreshold;
            set => m_ReleaseThreshold = value;
        }

        /// <summary>
        ///     See <see cref="MonoBehaviour" />.
        /// </summary>
        private void Update()
        {
            // Go true when either the press threshold is reached or the bool is already performed.
            // Only drop back to false when the release threshold is reached and the bool is no longer performed.
            var prevPerformed = m_IsPerformed;
            var pressAmount = m_ValueInput.ReadValue();

            bool newValue;
            if (prevPerformed)
                newValue = pressAmount > m_ReleaseThreshold || m_ValueInput.ReadIsPerformed();
            else
                newValue = pressAmount >= m_PressThreshold || m_ValueInput.ReadIsPerformed();

            m_IsPerformed = newValue;
            m_WasPerformedThisFrame = !prevPerformed && m_IsPerformed;
            m_WasCompletedThisFrame = prevPerformed && !m_IsPerformed;
        }

        /// <summary>
        ///     See <see cref="MonoBehaviour" />.
        /// </summary>
        private void OnEnable()
        {
            m_ValueInput?.EnableDirectActionIfModeUsed();
        }

        /// <summary>
        ///     See <see cref="MonoBehaviour" />.
        /// </summary>
        private void OnDisable()
        {
            m_ValueInput?.DisableDirectActionIfModeUsed();
        }

        /// <inheritdoc />
        public bool ReadIsPerformed()
        {
            return m_IsPerformed;
        }

        /// <inheritdoc />
        public bool ReadWasPerformedThisFrame()
        {
            return m_WasPerformedThisFrame;
        }

        /// <inheritdoc />
        public bool ReadWasCompletedThisFrame()
        {
            return m_WasCompletedThisFrame;
        }

        /// <inheritdoc />
        public float ReadValue()
        {
            return m_ValueInput.ReadValue();
        }

        /// <inheritdoc />
        public bool TryReadValue(out float value)
        {
            return m_ValueInput.TryReadValue(out value);
        }
    }
}
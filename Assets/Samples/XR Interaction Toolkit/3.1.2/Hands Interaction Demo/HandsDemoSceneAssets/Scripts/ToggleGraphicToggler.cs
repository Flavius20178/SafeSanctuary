using UnityEngine.UI;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    ///     Toggles between two graphic objects based on the state of a toggle.
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class ToggleGraphicToggler : MonoBehaviour
    {
        [SerializeField] [Tooltip("Graphic object representing the toggle on position.")]
        private Graphic m_ToggleOnGraphic;

        [SerializeField] [Tooltip("Graphic object representing the toggle off position.")]
        private Graphic m_ToggleOffGraphic;

        private Toggle m_TargetToggle;

        /// <summary>
        ///     See <see cref="MonoBehaviour" />.
        /// </summary>
        private void Awake()
        {
            m_TargetToggle = GetComponent<Toggle>();
        }

        /// <summary>
        ///     See <see cref="MonoBehaviour" />.
        /// </summary>
        private void OnEnable()
        {
            m_TargetToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        /// <summary>
        ///     See <see cref="MonoBehaviour" />.
        /// </summary>
        private void OnDisable()
        {
            m_TargetToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            if (isOn)
                m_TargetToggle.targetGraphic = m_ToggleOnGraphic;
            else
                m_TargetToggle.targetGraphic = m_ToggleOffGraphic;

            m_ToggleOnGraphic.gameObject.SetActive(isOn);
        }
    }
}
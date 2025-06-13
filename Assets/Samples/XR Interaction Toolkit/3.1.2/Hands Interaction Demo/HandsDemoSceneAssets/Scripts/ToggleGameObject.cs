namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    ///     Toggles the active state of a GameObject.
    /// </summary>
    public class ToggleGameObject : MonoBehaviour
    {
        [SerializeField] [Tooltip("The GameObject to toggle the active state for.")]
        private GameObject m_ActivationGameObject;

        [SerializeField] [Tooltip("Whether the GameObject is currently active.")]
        private bool m_CurrentlyActive;

        /// <summary>
        ///     The GameObject to toggle the active state for.
        /// </summary>
        public GameObject activationGameObject
        {
            get => m_ActivationGameObject;
            set => m_ActivationGameObject = value;
        }

        /// <summary>
        ///     Whether the GameObject is currently active.
        /// </summary>
        public bool currentlyActive
        {
            get => m_CurrentlyActive;
            set
            {
                m_CurrentlyActive = value;
                activationGameObject.SetActive(m_CurrentlyActive);
            }
        }

        /// <summary>
        ///     Toggles the active state of the GameObject.
        /// </summary>
        public void ToggleActiveState()
        {
            m_CurrentlyActive = !m_CurrentlyActive;
            activationGameObject.SetActive(m_CurrentlyActive);
        }
    }
}
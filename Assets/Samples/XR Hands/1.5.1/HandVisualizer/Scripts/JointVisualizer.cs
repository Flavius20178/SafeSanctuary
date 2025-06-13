namespace UnityEngine.XR.Hands.Samples.VisualizerSample
{
    public class JointVisualizer : MonoBehaviour
    {
        [SerializeField] private GameObject m_JointVisual;

        [SerializeField] private Material m_HighFidelityJointMaterial;

        [SerializeField] private Material m_LowFidelityJointMaterial;

        private bool m_HighFidelityJoint;

        private Renderer m_JointRenderer;

        private void Start()
        {
            if (m_JointVisual.TryGetComponent<Renderer>(out var jointRenderer))
                m_JointRenderer = jointRenderer;
        }

        public void NotifyTrackingState(XRHandJointTrackingState jointTrackingState)
        {
            var highFidelityJoint = (jointTrackingState & XRHandJointTrackingState.HighFidelityPose) ==
                                    XRHandJointTrackingState.HighFidelityPose;
            if (m_HighFidelityJoint == highFidelityJoint)
                return;

            m_JointRenderer.material = highFidelityJoint ? m_HighFidelityJointMaterial : m_LowFidelityJointMaterial;

            m_HighFidelityJoint = highFidelityJoint;
        }
    }
}
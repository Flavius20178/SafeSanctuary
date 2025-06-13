using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    ///     Class used to sync the transform of a target game object with this one.
    /// </summary>
    public class TransformSync : MonoBehaviour
    {
        [SerializeField] [Tooltip("Transform to apply this transform's data to.")]
        private Transform m_TargetTransform;

        [SerializeField] [Range(0f, 30f)] [Tooltip("Set to 0 for no smoothing. Higher values indicate more smoothing.")]
        private float m_SmoothFollowSpeed = 8f;

        private bool m_HasRigidbody;

        private bool m_HasTransform;

        private Rigidbody m_Rigidbody;

        private Transform m_ThisTransform;

        /// <summary>
        ///     See <see cref="MonoBehaviour" />.
        /// </summary>
        private void Awake()
        {
            m_ThisTransform = transform;
        }

        /// <summary>
        ///     See <see cref="MonoBehaviour" />.
        /// </summary>
        private void Update()
        {
            m_PositionTweenable.target = m_ThisTransform.position;
            m_RotationTweenable.target = m_ThisTransform.rotation;

            var tweenTarget = m_SmoothFollowSpeed > 0f ? m_SmoothFollowSpeed * Time.deltaTime : 1f;
            m_PositionTweenable.HandleTween(tweenTarget);
            m_RotationTweenable.HandleTween(tweenTarget);

            if (!m_HasRigidbody && m_HasTransform)
                m_TargetTransform.SetPositionAndRotation(m_PositionTweenable.Value, m_RotationTweenable.Value);
        }

        /// <summary>
        ///     See <see cref="MonoBehaviour" />.
        /// </summary>
        private void FixedUpdate()
        {
            if (!m_HasRigidbody)
                return;

            m_Rigidbody.MovePosition(m_PositionTweenable.Value);
            m_Rigidbody.MoveRotation(m_RotationTweenable.Value);
        }

        /// <summary>
        ///     See <see cref="MonoBehaviour" />.
        /// </summary>
        private void OnEnable()
        {
            if (m_TargetTransform == null)
            {
                enabled = false;
                return;
            }

            m_HasTransform = true;

            if (m_TargetTransform.TryGetComponent(out Rigidbody rigidBodyComponent))
            {
                m_Rigidbody = rigidBodyComponent;
                m_HasRigidbody = true;
            }

            m_PositionTweenable.Value = m_ThisTransform.position;
            m_RotationTweenable.Value = m_ThisTransform.rotation;
        }

        /// <summary>
        ///     See <see cref="MonoBehaviour" />.
        /// </summary>
        private void OnValidate()
        {
            if (m_TargetTransform != null)
                transform.localPosition = transform.parent == null
                    ? m_TargetTransform.position
                    : transform.parent.InverseTransformPoint(m_TargetTransform.position);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        private readonly Vector3TweenableVariable m_PositionTweenable = new();
        private readonly QuaternionTweenableVariable m_RotationTweenable = new();
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
using UnityEngine;

namespace Vertigo2
{
    /// <summary>
    ///     Vertigo Water. Is a flat finite volume of water
    /// </summary>
    public class VWater : VWaterBase
    {
        [Tooltip("Move the water plane up or down. You probably should leave this at zero")]
        public float surfaceOffset;

        [HideInInspector] public VWaterRenderer linkedRenderer;

        // all water trigger zones. Used for this body of water's bounds
        private Collider[] m_cols;

        public float Position => transform.position.y + surfaceOffset * transform.lossyScale.y;

        private void Start()
        {
            m_cols = GetComponentsInChildren<Collider>();
            if (m_cols.Length > 0)
            {
                //create bounds combining all collider bounds
                bounds = m_cols[0].bounds;
                for (var i = 1; i < m_cols.Length; i++)
                {
                    bounds.Encapsulate(m_cols[i].bounds.max);
                    bounds.Encapsulate(m_cols[i].bounds.min);
                }
            }
        }

        public override bool CheckForExitWater(VWaterBody b)
        {
            for (var i = 0; i < m_cols.Length; i++)
                if (m_cols[i].bounds.Intersects(b.mainCollider.bounds))
                    return false;

            return true;
        }

        public bool IsPointInWater(Vector3 point)
        {
            var touching = false;
            for (var i = 0; i < m_cols.Length; i++)
                if (m_cols[i].bounds.Contains(point))
                    touching = true;
            return touching;
        }
    }
}
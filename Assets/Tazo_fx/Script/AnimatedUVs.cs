using UnityEngine;

namespace TazoScript
{
    public class AnimatedUVs : MonoBehaviour
    {
        public float speedY = 0.5F;
        public float speedx;
        private float offsetx;
        private float offsety;
        private Renderer rend;

        private void Start()
        {
            rend = GetComponent<Renderer>();
        }

        private void Update()
        {
            offsety += Time.deltaTime * speedY;
            offsetx += Time.deltaTime * speedx;
            rend.material.SetTextureOffset("_MainTex", new Vector2(offsetx, offsety));
        }
    }
}
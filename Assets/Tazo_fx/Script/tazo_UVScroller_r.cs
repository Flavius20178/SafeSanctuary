using UnityEngine;

namespace TazoScript
{
    public class tazo_UVScroller_r : MonoBehaviour
    {
        public int targetMaterialSlot;
        public float speedY = 0.5f;
        public float speedX;
        private Renderer myrender;
        private float timeWentX;
        private float timeWentY;

        private void Start()
        {
            myrender = GetComponent<Renderer>();
        }

        private void Update()
        {
            timeWentY += Time.deltaTime * speedY;
            timeWentX += Time.deltaTime * speedX;
            myrender.material.SetTextureOffset("_DistortTex", new Vector2(timeWentX, timeWentY));
            myrender.material.SetTextureOffset("_RefractionLayer", new Vector2(timeWentX, timeWentY));
        }

        //void OnEnable (){
        //
        //		myrender.material.SetTextureOffset ("_MainTex",new Vector2(0, 0));
        //		timeWentX = 0;
        //		timeWentY = 0;
        //}
    }
}
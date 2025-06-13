using UnityEngine;

namespace TazoScript
{
    public class BillBoard : MonoBehaviour
    {
        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
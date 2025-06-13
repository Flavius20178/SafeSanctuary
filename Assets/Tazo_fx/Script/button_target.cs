using UnityEngine;
using UnityEngine.UI;

namespace TazoScript
{
    public class button_target : MonoBehaviour
    {
        public GameObject MY_target;
        private GameObject[] ALL_target;
        private GameObject temp_target;

        // Use this for initialization
        private void Start()
        {
            transform.GetChild(0).GetComponent<Text>().text = MY_target.name;
            //print (this.transform.GetChild (0).GetComponent<Text>().text);
            if (ALL_target == null)
                ALL_target = GameObject.FindGameObjectsWithTag("TAZOFX");
            foreach (var tt in ALL_target) tt.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void ShowTarget()
        {
            ALL_target = GameObject.FindGameObjectsWithTag("TAZOFX");
            foreach (var tt in ALL_target) tt.SetActive(false);

            MY_target.SetActive(true);
        }
    }
}
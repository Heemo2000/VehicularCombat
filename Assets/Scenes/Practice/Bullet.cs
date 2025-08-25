using UnityEngine;

namespace Game
{
    public class Bullet : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("enemy touch");
            if (other.gameObject.tag == "enemy")
            {
               
                Debug.Log("health - ");
            }
        }
    }
}

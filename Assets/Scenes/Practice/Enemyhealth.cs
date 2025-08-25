using UnityEngine;

    public class Enemyhealth : MonoBehaviour
    {
        [Header("Health stats")]
        public int health;
        public int bulletdamage;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }

       
    
    }

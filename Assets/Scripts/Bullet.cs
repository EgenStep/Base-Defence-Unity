using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDefence
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        public float damage;
        [SerializeField]
        float lifeTime;

        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Enemy")
            {
                Destroy(gameObject, 0.0f);
            }
        }

        
    }
}

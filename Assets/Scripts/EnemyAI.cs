using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDefence
{
    public class EnemyAI : MonoBehaviour
    {
        enum State { walk, follow, attack, die }
        //[SerializeField]
        public GameObject player;
        [SerializeField]
        GameObject bitObj;
        public float playerDetectDistance;
        public float maxVelosity;
        public float maxHealth;
        [HideInInspector]
        public float health;
        public float damage;
        [SerializeField]
        float damageTime;
        Vector3 velocity;
        State state;
        Rigidbody rb;
        //SphereCollider sc;
        [SerializeField]
        float hitPlayerDist;
        bool animatingAttack = false;
        [SerializeField]
        GameObject[] items;
        Vector3 targetWalk;
        public Vector3 targetBase;
        //bool isAttacking = false;

        // Start is called before the first frame update
        void Start()
        {
            state = State.walk;            
            rb = GetComponent<Rigidbody>();
            velocity = new Vector3(0f, 0f, -maxVelosity);
            health = maxHealth;
            bitObj = transform.Find("Bat").gameObject;
            player.GetComponent<PlayerAI>().EnemySpawn(gameObject);
        }

        private void FixedUpdate()
        {            
            rb.velocity = VelocityDependOnState();
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }

        Vector3 VelocityDependOnState()
        {
            Vector3 velocity = Vector3.zero;
            if(state==State.die) return velocity;
            if(state == State.attack) return velocity;
            if(state != State.walk)
            {
                var directionToBase = (targetBase - transform.position).normalized;
                velocity = directionToBase * maxVelosity;
                if ((player.transform.position - transform.position).magnitude < playerDetectDistance) state = State.follow;
            }
            if (state == State.follow)
            {
                var directionToPlayer = (player.transform.position - transform.position).normalized;
                velocity = directionToPlayer * maxVelosity;
                if ((player.transform.position - transform.position).magnitude > playerDetectDistance) state = State.walk;
            }
            return velocity;
        }

        IEnumerator Death()
        {            
            state = State.die;
            gameObject.layer = 11;
            gameObject.tag = "DefeatedEnemy";
            transform.localScale = new Vector3(1f, 0.2f, 1f);
            GetComponent<Renderer>().material.color = Color.gray;
            rb.constraints = RigidbodyConstraints.None;
            yield return new WaitForSeconds(0.5f);
            DropItem();
            yield return new WaitForSeconds(0.0f);
            player.GetComponent<PlayerAI>().EnemyDeath(gameObject);
            Destroy(gameObject);
            yield return null;
        }

        void DropItem()
        {
            int rnd = Random.Range(0, 2);
            player.GetComponent<PlayerAI>().DropItem(rnd);
            var item = Instantiate(items[rnd], transform.position, items[rnd].transform.rotation);
            item.GetComponent<Item>().type = (Item.Type)rnd;
        }

        void HitPlayer()
        {
            if (gameObject.tag != "Enemy") return;
            float distance = (player.transform.position - transform.position).magnitude;
            if (distance > hitPlayerDist) return;
            player.GetComponent<PlayerAI>().ReceiveDamage(damage);
        }

        IEnumerator AnimateAttack()
        {
            animatingAttack = true;
            while (animatingAttack)
            {
                float t = 0f;
                float dT = 0.1f;
                while (true)
                {
                    t += dT;
                    float angle = (1f - Mathf.Sin(t / damageTime * Mathf.PI)) * 45f;
                    bitObj.transform.localEulerAngles = new Vector3(angle, 0, 0);
                    yield return new WaitForSeconds(dT);
                    if (t >= damageTime) break;
                }
                HitPlayer();
                yield return new WaitForSeconds(dT);
            }
            
        }
       
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Bullet")
            {
                float damage = other.gameObject.GetComponent<Bullet>().damage;
                health -= damage;
                if (health < 0f) StartCoroutine(Death());
            }
            if (other.gameObject.tag == "Player" && !animatingAttack) StartCoroutine(AnimateAttack());
        }        
    }
}

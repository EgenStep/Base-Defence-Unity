using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDefence
{
    public class EnemyAnimated : MonoBehaviour
    {
        enum State { walk, follow, attack, die }
        //[SerializeField]
        public GameObject player;
        [SerializeField]
        float playerDetectDistance;
        [SerializeField]
        float maxVelosity;
        [SerializeField]
        float attackVelosity;
        public float maxHealth;
        [HideInInspector]
        public float health;
        public float damage;
        [SerializeField]
        float damageTime;
        Vector3 velocity;
        State state;
        State nextState;
        Rigidbody rb;
        //SphereCollider sc;
        [SerializeField]
        float hitPlayerDist;
        [SerializeField]
        float startAttackDist;
        [SerializeField]
        GameObject[] items;
        //Vector3 targetWalk;
        [HideInInspector]
        internal Vector3 targetBase;
        Animator animator;
        int itemsN;
        GameObject batObj;
        bool isAttacking = false;
        Material material;

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            state = State.walk;
            rb = GetComponent<Rigidbody>();
            velocity = new Vector3(0f, 0f, -maxVelosity);
            health = maxHealth;
            player.GetComponent<PlayerAI>().EnemySpawn(gameObject);
            itemsN = player.GetComponent<PlayerAI>().itemsN;
            batObj = transform.Find("EnemyVoodooCustom/VoodooCustomCharacterV1_3/mixamorig:Hips/mixamorig:Spine/mixamorig:" +
                "Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/Bat").gameObject;
            material = transform.Find("EnemyVoodooCustom/VoodooCustomCharacterV1_3/VoodooCustomCharacter_1").gameObject.GetComponent<Renderer>().material;
        }

        private void FixedUpdate()
        {
            SetStateOnRunTime();
            rb.velocity = VelocityDependOnState();
            if (velocity != Vector3.zero && state != State.die)  
                transform.rotation = Quaternion.LookRotation(rb.velocity);
        }

        void SetStateOnRunTime()
        {
            if (state == State.die) return;
            if (player.tag == "DefeatedPlayer")
            {
                state = State.walk;
                return;
            }
            var distanceToPlayer = (player.transform.position - transform.position).magnitude;
            if (distanceToPlayer > playerDetectDistance) state = State.walk;
            if (distanceToPlayer < playerDetectDistance) state = State.follow;
            if (distanceToPlayer < startAttackDist) state = State.attack;
            if (state == State.attack && !isAttacking) StartCoroutine(OneAttack());
            if (state == State.walk || state == State.follow) animator.SetInteger("state", 0);
        }

        IEnumerator OneAttack()
        {
            isAttacking = true;
            animator.SetInteger("state", 1);
            yield return new WaitForSeconds(damageTime);
            var distanceToPlayer = (player.transform.position - transform.position).magnitude;
            if(distanceToPlayer < hitPlayerDist)
            {
                player.GetComponent<PlayerAI>().ReceiveDamage(damage);
            }
            isAttacking = false;
        }

        Vector3 VelocityDependOnState()
        {
            velocity = Vector3.zero;
            if (state == State.die) return velocity;
            var directionToPlayer = (player.transform.position - transform.position).normalized;
            directionToPlayer.y = 0f;
            if (state == State.attack)
            {                
                velocity = directionToPlayer * attackVelosity;
            }
            if (state == State.walk)
            {
                var directionToBase = (targetBase - transform.position).normalized;
                directionToBase.y = 0f;
                velocity = directionToBase * maxVelosity;
                //if ((player.transform.position - transform.position).magnitude < playerDetectDistance) state = State.follow;
            }
            if (state == State.follow)
            {                
                velocity = directionToPlayer * maxVelosity;
                //if ((player.transform.position - transform.position).magnitude > playerDetectDistance) state = State.walk;
            }
            return velocity;
        }

        IEnumerator Death()
        {
            state = State.die;
            animator.SetInteger("state", 2);
            gameObject.layer = 11;
            gameObject.tag = "DefeatedEnemy";
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            player.GetComponent<PlayerAI>().EnemyDeath(gameObject);
            //rb.constraints = RigidbodyConstraints.None;
            yield return new WaitForSeconds(0.7f);
            material.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);
            batObj.transform.localEulerAngles = new Vector3(0, 0, 135);
            DropItem();
            yield return new WaitForSeconds(0.0f);
            Destroy(gameObject, 3f);
            yield return null;
        }

        void DropItem()
        {
            int rnd = Random.Range(0, itemsN);
            player.GetComponent<PlayerAI>().DropItem(rnd);
            var item = Instantiate(items[rnd], transform.position, items[rnd].transform.rotation);
            item.GetComponent<Item>().type = (Item.Type)rnd;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Bullet")
            {
                float damage = other.gameObject.GetComponent<Bullet>().damage;
                health -= damage;
                if (health < 0f) StartCoroutine(Death());
            }
            //if (other.gameObject.tag == "Player" && !animatingAttack) StartCoroutine(AnimateAttack());
        }
    }
}

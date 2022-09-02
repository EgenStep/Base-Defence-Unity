using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BasDefence
{
    public class PlayerAI : MonoBehaviour
    {
        enum Anim {idle = 0,run = 1}
        Anim anim = Anim.idle;
        [SerializeField]
        GameObject startBullet;
        [SerializeField]
        float bulletVelosity;
        [SerializeField]
        private float enemyDetectDistance;
        [HideInInspector]
        public GameObject targetEnemy = null;
        float targetDist;
        [SerializeField]
        float targetToNearestChangeDistance;
        GameObject nearestEnemy;
        float nearestDist;
        [SerializeField]
        GameObject bulletPrefab;
        [SerializeField]
        float fireN;
        SphereCollider sc;
        [SerializeField]
        float maxHealth;
        float health;
        [SerializeField]
        float restoreHealthOnBase;
        [SerializeField]
        float maxVelosity;
        [SerializeField]
        Text healthText;
        [SerializeField]
        Slider healthSlider;
        [SerializeField]
        GameObject joystick;
        [SerializeField]
        GameObject keyboard;
        Vector3 joystickDirection;
        Rigidbody rb;
        [SerializeField]
        Camera cam;
        Vector3 camShift;
        public bool onBaseTerritory = false;
        int defeatedEnemies = 0;
        List<int> itemsDrop = new List<int>();
        List<int> itemsBag = new List<int>();
        List<int> itemsInventory = new List<int>();
        [SerializeField]
        GameObject bagObject;
        [SerializeField]
        int bagCount;
        int bagIndex = 0;
        [SerializeField]
        float bagStepY;
        [SerializeField]
        float scaleItemInBag;
        [SerializeField]
        Text[] textBag;
        [SerializeField]
        Text[] textInventory;
        List<GameObject> enemyObjects = new List<GameObject>();
        List<GameObject> bagItemObjects = new List<GameObject>();
        [HideInInspector]
        internal int itemsN;
        Animator animator;
        [SerializeField]
        Transform rifle;
        Vector3 idlePos = new Vector3(0.054f, -0.003f, 0.053f);
        Vector3 idleRot = new Vector3(-61f, 90f, 0f);
        [SerializeField]
        Transform pistol;
        bool restoreHealth = false;
        Vector3 prevDirection;
        [SerializeField]
        GameObject imageDeath;
        bool death = false;
        Vector3 posInit;
        Vector3 WASD;

        // Start is called before the first frame update
        void Start()
        {
            posInit = transform.position;
            animator = GetComponentInChildren<Animator>();            
            SetAllTypesOfItems();
            health = maxHealth;
            rb = gameObject.GetComponent<Rigidbody>();
            camShift = cam.transform.position - transform.position;
            imageDeath.SetActive(false);
            RefreshHealthBar();           
            StartCoroutine(Shooting());
        }

        void SetAllTypesOfItems()
        {
            itemsN = Enum.GetValues(typeof(Item.Type)).Length;
            for (int i = 0; i < itemsN; i++)
            {
                itemsDrop.Add(0);
                itemsBag.Add(0);
                itemsInventory.Add(0);
            }
        }

        private void FixedUpdate()
        {
            if (death) return;
            joystickDirection = joystick.GetComponent<Joystick>().direction;
            if (joystickDirection.magnitude < 1f)
            {
                joystickDirection = keyboard.GetComponent<KeyBoard>().direction;
            }
            //Debug.Log(joystickDirection);
            if (joystickDirection.magnitude < 1f) anim = Anim.idle;
            else anim = Anim.run;
            animator.SetInteger("state", (int)anim);
            joystickDirection = new Vector3(joystickDirection.x, 0f, joystickDirection.y);
            if (joystickDirection.magnitude > 0f) prevDirection = joystickDirection;
            rb.velocity = joystickDirection.normalized * maxVelosity;
        }
        
        IEnumerator Shooting()
        {
            float shootTime = 1f / fireN;
            while (!death)
            {
                if (targetEnemy != null && !onBaseTerritory)  
                {
                    if (targetEnemy.tag != "Enemy")
                    {
                        targetEnemy = null;
                        continue;
                    }
                    float dist = (targetEnemy.transform.position - transform.position).magnitude;
                    if (dist > enemyDetectDistance)
                    {
                        targetEnemy = null;
                        continue;
                    }
                    Vector3 direction = targetEnemy.transform.position - startBullet.transform.position + Vector3.up * 0.0f;
                    direction.y = 0f;
                    Vector3 velosity = direction.normalized * bulletVelosity;
                    var bullet = Instantiate(bulletPrefab, startBullet.transform.position, Quaternion.identity);
                    var rb = bullet.GetComponent<Rigidbody>();
                    rb.velocity = velosity;
                }
                onBaseTerritory = false;
                yield return new WaitForSeconds(shootTime);
            }            
        }
         
        // Update is called once per frame
        void Update()
        {
            if (death) return;
            SetTargetEnemy();
            if (targetEnemy != null)
            {
                Vector3 direction = targetEnemy.transform.position - startBullet.transform.position;
                direction.y = 0f;   
                prevDirection = direction;
                transform.rotation = Quaternion.LookRotation(direction);
                pistol.rotation = Quaternion.LookRotation(direction);
            }
            else
            {
                if (joystickDirection.magnitude > 0f) transform.rotation = Quaternion.LookRotation(rb.velocity);
                else transform.rotation = Quaternion.LookRotation(prevDirection);
            }            
            cam.transform.position = transform.position + camShift;
        }

        IEnumerator RestoreHealthOnBase()
        {
            restoreHealth = true;
            while (health < maxHealth) 
            {
                yield return new WaitForSeconds(1f);
                if (onBaseTerritory)
                {
                    health += restoreHealthOnBase;
                    RefreshHealthBar();
                }
                else break;
            }
            if (health > maxHealth) health = maxHealth;
            restoreHealth = false;
            RefreshHealthBar();
        }

        void SetTargetEnemy()
        {
            SetNearestEnemy();
            if (nearestEnemy != null)
            {
                if (targetEnemy == null)
                {
                    targetEnemy = nearestEnemy;
                }
                else
                {
                    if (targetDist - nearestDist > targetToNearestChangeDistance) targetEnemy = nearestEnemy;
                }
            }
        }

        void SetNearestEnemy()
        {
            nearestDist = float.MaxValue;
            nearestEnemy = null;
            foreach (var enemy in enemyObjects)
            {
                Vector3 direction = enemy.transform.position - transform.position;
                direction.y = 0f;
                float dist = direction.magnitude;
                if (targetEnemy != null && targetEnemy == enemy) targetDist = dist;                
                if (dist < nearestDist && dist <= enemyDetectDistance)
                {
                    nearestDist = dist;
                    nearestEnemy = enemy;
                }
            }
        }
                
        private void OnTriggerEnter(Collider other)
        {
            if (death) return;
            var obj = other.gameObject;
            Debug.Log("player " + obj.tag.ToString());
            if (obj.tag == "Item") PickUpItem(obj);
            if (obj.tag == "BaseTerritory") BagOnBase();            
        }

        private void OnTriggerStay(Collider other)
        {
            if (death) return;
            if (other.gameObject.tag == "BaseTerritory")
            {
                onBaseTerritory = true;
                if (!restoreHealth && health < maxHealth) StartCoroutine(RestoreHealthOnBase());
            }
        }

        void BagOnBase()
        {
            bagIndex = 0;
            for (int i = 0; i < itemsN; i++)
            {
                itemsInventory[i] += itemsBag[i];
                itemsBag[i] = 0;
            }
            RefreshItemText();
            StartCoroutine(BagOnBaseAnimated());    
        }

        IEnumerator BagOnBaseAnimated()
        {
            var buffer = bagItemObjects.ToArray();
            bagItemObjects.Clear();
            float dT = 0.1f;
            int i = buffer.Length - 1;
            while(i >= 0)
            {
                Destroy(buffer[i]);
                i--;
                yield return new WaitForSeconds(dT);
            }
        }

        void PickUpItem(GameObject item)
        {
            if (bagIndex >= bagCount) return;
            bagIndex++;
            int i = bagIndex;
            bagItemObjects.Add(item);         
            item.GetComponent<Collider>().enabled = false;
            item.GetComponent<Item>().enabled = false;
            item.transform.eulerAngles = new Vector3(item.transform.eulerAngles.x + 90f, 0f, 0f);
            var type = item.GetComponent<Item>().type;
            int index = (int)type;
            itemsBag[index]++;
            textBag[index].text = itemsBag[index].ToString();
            StartCoroutine(ItemFlyToBag(item, i));
        }

        void RefreshItemText()
        {
            for (int i = 0; i < itemsN; i++)
            {
                textBag[i].text = itemsBag[i].ToString();
                textInventory[i].text = itemsInventory[i].ToString();
            }
        }

        void RefreshHealthBar()
        {
            healthText.text = health.ToString();
            healthSlider.value = health / maxHealth;
        }

        IEnumerator ItemFlyToBag(GameObject item, int index)
        {
            var scaleStart = item.transform.localScale;
            var scaleEnd = item.transform.localScale * scaleItemInBag;
            float dT = 0.05f;
            float time = 0f;
            float duration = 0.8f;
            var start = item.transform.position;
            while(time < duration)
            {
                float x = time / duration;
                var end = bagObject.transform.position + Vector3.up * index * bagStepY * scaleItemInBag;
                var coord = start + (end - start) * x;
                float y = (1f - 4f * (x - 0.5f) * (x - 0.5f)) * 3f;
                coord += Vector3.up * y;
                var scale = Vector3.Lerp(scaleStart, scaleEnd, x);
                if (item != null)
                {
                    item.transform.position = coord;
                    item.transform.localScale = scale;
                }
                time += dT;
                yield return new WaitForSeconds(dT);
            } 
            if(item!= null)
            {
                item.transform.parent = bagObject.transform;
                item.transform.localPosition = Vector3.up * (float)index * bagStepY;
            }            
        }

        internal void DropItem(int type)
        {
            itemsDrop[type]++;
            //Debug.Log("Items 0 " + itemsDrop[0] + " Items 1 " + itemsDrop[1]);
        }

        internal void EnemySpawn(GameObject enemy)
        {
            enemyObjects.Add(enemy);
        }

        internal void EnemyDeath(GameObject enemy)
        {
            enemyObjects.Remove(enemy);
        }

        internal void DefeatEnemy()
        {
            defeatedEnemies++;
        }

        internal void ReceiveDamage(float damage)
        {
            if (death) return;
            health -= damage;
            RefreshHealthBar();
            if (health <= 0)
            {
                death = true;
                StartCoroutine(Death());
            }
        }

        IEnumerator Death()
        {            
            gameObject.tag = "DefeatedPlayer";
            gameObject.layer = 15;            
            animator.SetInteger("state", 2);
            yield return new WaitForSeconds(2f);
            for (int i = 0; i < itemsN; i++)
            {
                itemsBag[i] = 0;
            }
            RefreshItemText();
            float dT = 0.1f;
            foreach (var item in bagItemObjects)
            {
                yield return new WaitForSeconds(dT);
                DropItem(item);
            }
            bagItemObjects.Clear();
            yield return new WaitForSeconds(2f);
            imageDeath.SetActive(true);
            yield return null;
        }

        void DropItem(GameObject item)
        {            
            item.GetComponent<Collider>().enabled = true;
            item.GetComponent<Item>().enabled = true;
            item.GetComponent<Item>().PlayerDeath(scaleItemInBag);
        }

        public void RestartAfterDeath()
        {
            imageDeath.SetActive(false);
            health = maxHealth;
            RefreshHealthBar();
            transform.position = posInit;
            gameObject.tag = "Player";
            gameObject.layer = 8;
            animator.SetInteger("state", 0);
            death = false;
            StartCoroutine(Shooting());
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}

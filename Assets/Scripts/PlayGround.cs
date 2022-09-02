using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BasDefence
{
    public class PlayGround : MonoBehaviour
    {
        public GameObject enemyPrefab;
        public GameObject playerObj;
        //private PlayerData playerData;
        public GameObject AllBounds;
        public float spawntime;
        float time = 0f;
        Vector3 minPoint;
        Vector3 maxPoint;
        float gateZ;
        [SerializeField]
        float spawnZ;
        float gateLength;
        List<GameObject> enemies = new List<GameObject>();
        GameObject targetEnemy;
        [SerializeField]
        float enemyDetectDistance;


        // Start is called before the first frame update
        void Start()
        {
            minPoint = AllBounds.GetComponent<AllBounds>().minPoint;
            maxPoint = AllBounds.GetComponent<AllBounds>().maxPoint;
            gateZ = AllBounds.GetComponent<AllBounds>().gateZ;
            gateLength = AllBounds.GetComponent<AllBounds>().gateLength;
            //playerData = new PlayerData();
            //StartCoroutine(ActivateGun());
        }

        private void Update()
        {
            SpawnEnemy();
            SetPlayerTargetEnemy();
        }       
        
        void SetPlayerTargetEnemy()
        {
            targetEnemy = null;
            float min = float.MaxValue;
            int n = enemies.Count - 1;
            for (int i = n; i >= 0; i--) 
            {
                var enemy = enemies[i];
                if (enemy == null)
                {
                    enemies.RemoveAt(i);
                    continue;
                }
                if (enemy.tag != "Enemy") continue;
                float dist = (enemy.transform.position - playerObj.transform.position).magnitude;
                if (dist < min) 
                {
                    min = dist;
                    if (dist <= enemyDetectDistance) targetEnemy = enemy;
                }
            }
            playerObj.GetComponent<PlayerAI>().targetEnemy = targetEnemy;
        }

        void SpawnEnemy()
        {
            time += Time.deltaTime;
            if (time < spawntime) return;
            time = 0f;
            float x = Random.Range(minPoint.x, maxPoint.x);
            float z = Random.Range(gateZ + spawnZ, maxPoint.z);
            float middleX = (minPoint.x + maxPoint.x) / 2f;
            float tagX = Random.Range(minPoint.x, middleX - gateLength / 2f - 2f);
            if (x > middleX) tagX = -tagX;
            var coord = new Vector3(x, 0f, z);
            var enemy = Instantiate(enemyPrefab, coord, Quaternion.identity);
            enemy.GetComponent<EnemyAnimated>().player = playerObj;
            enemy.GetComponent<EnemyAnimated>().targetBase = new Vector3(tagX, 0f, gateZ);
            enemies.Add(enemy);
        }

        //public void SliderDrag()
        //{
        //    float value = sliderDrag.value;
        //    float min = 0f;
        //    float max = 1f;
        //    float drag = min + (max - min) * value;
        //    sliderDragText.text = "Drag " + drag;
        //}
 
    }
}
